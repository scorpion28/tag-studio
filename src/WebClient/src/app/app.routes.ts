import { Routes } from '@angular/router';
import { TagsPage } from './features/tag/tags-page.component';
import { MainLayoutComponent } from './core/layout/main-layout/main-layout.component';
import { AuthComponent } from './core/auth/auth.component';
import { authGuard } from './core/auth/services/auth.guard';
import { EntriesPage } from './features/entry/entries.page';

export const routes: Routes = [
  { path: '', redirectTo: "/app", pathMatch: 'full' },
  {
    path: 'app',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: 'tags', component: TagsPage, pathMatch: "prefix" },
      { path: 'entries', component: EntriesPage, pathMatch: "prefix" },
      {
        path: '',
        redirectTo: 'tags',
        pathMatch: 'full'
      }
    ]
  },
  { path: 'login', component: AuthComponent, title: 'Log In | TagStudio' },
  { path: 'signup', component: AuthComponent, title: 'Sign Up | TagStudio' }
];
