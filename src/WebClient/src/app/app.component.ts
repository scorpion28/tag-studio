import { Component } from '@angular/core';
import {RouterOutlet} from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  template: `
    <div id="root" class="h-full">
      <router-outlet></router-outlet>
    </div>
  `,
})
export class AppComponent {
}
