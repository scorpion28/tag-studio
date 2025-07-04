import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SearchBarComponent } from './search-bar/search-bar.component';

@Component({
  selector: 'top-bar',
  standalone: true,
  imports: [CommonModule, RouterModule, SearchBarComponent],
  templateUrl: './top-bar.component.html',
})
export class TopBarComponent {
  pageTitle = input("");
}
