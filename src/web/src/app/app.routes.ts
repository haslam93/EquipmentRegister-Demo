import { Routes } from '@angular/router';
import { ImportPage } from './import/import-page';
import { RegisterPage } from './register/register-page';

export const routes: Routes = [
  { path: '', component: RegisterPage, title: 'Equipment Register' },
  { path: 'import', component: ImportPage, title: 'Import - Equipment Register' },
  { path: '**', redirectTo: '' },
];
