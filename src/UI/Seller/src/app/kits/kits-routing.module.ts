// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { KitsTableComponent } from './components/kits-table/kits-table.component';

const routes: Routes = [
    { path: '', component: KitsTableComponent },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class KitsRoutingModule { }
