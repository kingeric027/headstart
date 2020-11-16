import { Injectable } from '@angular/core';
import domtoimage from 'dom-to-image';
import html2canvas from 'html2canvas';
import jspdf from 'jspdf';

@Injectable({
  providedIn: 'root',
})
export class PDFService {
  html: string;

  constructor() {}
  
  createAndSavePDF(orderID: string): void {
    const orderDetailObjectByID = document.getElementById('order-detail-pdf-range');
    this.removeNodesOfClass(orderDetailObjectByID, 'd-print-none');
    const pdfArea = document.createElement('div');
    pdfArea.classList.add('hidden-print-area');
    pdfArea.appendChild(orderDetailObjectByID);
    const links = pdfArea.getElementsByClassName('link-text');
    Array.from(links).forEach(link => {
      link.classList.add('pdf-links');
    });
    document.body.appendChild(pdfArea);
    const printObj = document.getElementById('order-detail-pdf-range');
    this.generateImagePDF(orderDetailObjectByID, printObj, orderID);
  }

  private generateImagePDF(orderDetailObject: any, printObj: any, orderID: string): void {
    domtoimage
      .toPng(printObj).then(dataUrl => {
        const img = new Image();
        img.src = dataUrl;
        img.onload = (): void => {
          const width = img.width;
          const height = img.height;
          const aspectRatio = width / height;
          const pdfHeight = 170 / aspectRatio;
          const pdf = new jspdf();
          pdf.addImage(dataUrl, 'PNG', 20, 20, 170, 170 / aspectRatio);
          const pages = Math.ceil(pdfHeight / 300);
          if (pages > 1) {
            let prevPageHeight = 0;
            for (let i = 1; i < pages; i++) {
              const newY = i === 1 ? 20 - pdf.internal.pageSize.height : prevPageHeight - pdf.internal.pageSize.height;
              pdf.addPage();
              pdf.addImage(dataUrl, 'PNG', 20, newY, 170, pdfHeight);
              prevPageHeight = newY;
            }
          }
          pdf.save(orderID + '.pdf');
          this.removeNodesOfClass(document, 'hidden-print-area');
        };
      })
      .catch(e => {this.generateNoImagePDF(orderDetailObject, printObj, orderID);}
      );
  }

  private async generateNoImagePDF(orderDetailObject: any, printObj: any, orderID: string): Promise<void> {
    this.removeNodesOfClass(orderDetailObject, 'img-thumbnail');
    const canvas = await html2canvas(printObj, { allowTaint: true });
    const pdf = new jspdf();
    const width = canvas.width;
    const height = canvas.height;
    const aspectRatio = width / height;
    const imgData = canvas.toDataURL('image/png');
    pdf.addImage(imgData, 'PNG', 20, 20, 170, 170 / aspectRatio);
    pdf.save(orderID + '.pdf');
    this.removeNodesOfClass(document, 'hidden-print-area');
  }

  private removeNodesOfClass(parentObject: any, classToRemove: string): void {
    if (parentObject && parentObject.childNodes) {
      parentObject.childNodes.forEach(childNode => {
        if (childNode.classList && Array.from(childNode.classList).includes(classToRemove)) {
          parentObject.removeChild(childNode);
        } else {
          this.removeNodesOfClass(childNode, classToRemove);
        }
      });
    }
  }
}
