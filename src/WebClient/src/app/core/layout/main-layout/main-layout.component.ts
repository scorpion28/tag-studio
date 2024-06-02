import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent } from '../sidebar/sidebar.component';

@Component({
  selector: 'app-main-layout',
  imports: [
    RouterOutlet,
    SidebarComponent
  ],
  template: `
    <div id="root" class="h-full">
      <sidebar />

      <main id="main-content" class="bg-dark-gray-900 h-full">
        <router-outlet></router-outlet>
      </main>

    </div>
  `,
  styles: `
    main {
      flex-grow: 1;
      margin-left: 200px;
      padding: 20px 40px 0 50px;
      width: calc(100% - 200px);
    }
  `
})
export class MainLayoutComponent {}
