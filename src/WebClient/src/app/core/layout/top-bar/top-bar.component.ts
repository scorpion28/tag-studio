import {Component, input} from '@angular/core';

@Component({
  selector: 'top-bar',
  imports: [],
  templateUrl: './top-bar.component.html',
})
export class TopBarComponent {
  pageTitle = input("")
}
