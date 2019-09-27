export class ModalState {
  static get Open() {
    return { isOpen: true };
  }
  static get Closed() {
    return { isOpen: false };
  }
}
