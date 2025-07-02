import { Component, input } from '@angular/core';
import { Entry } from '../models/entry.model';
import { TagListComponent } from '../../tag/tag-list.component';

@Component({
  selector: 'app-entry-card',
  template: `
    <div class="entry-card cursor-pointer">
      @if(entry().imageUrl; as image){
        <img [src]="image" class="card-image" alt="Entry image"/>
      } @else {
        <div class="card-image"></div>
      }

      <div class="card-content">
        <p class="text-alpha-81 font-medium">{{ entry().name }}</p>

        <div class="mt-2 overflow-hidden h-7">
          <app-tag-list [tags]="entry().tags" [wrap]="true" />
        </div>
      </div>
    </div>
  `,
  imports: [
    TagListComponent
  ],
  styles: `
    .entry-card {
      flex: 1 1 300px;
      max-width: 100%;
      width: 300px;
      height: 100%;

      border-radius: 8px;
      box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
      overflow: hidden;
      display: flex;
      flex-direction: column;
      background-color: var(--color-dark-gray-800);
    }

    .card-image {
      width: 100%;
      height: 200px;
      object-fit: cover;
    }

    .card-content {
      padding: 15px;
      flex-grow: 1;
      display: flex;
      background-color: var(--color-dark-gray-350);
      flex-direction: column;
      justify-content: flex-start;
    }

    @media (max-width: 768px) {
      .entry-card {
        flex: 1 1 calc(50% - 40px);
      }
    }

    @media (max-width: 480px) {
      .entry-card {
        flex: 1 1 calc(100% - 40px);
      }
    }
  `
})
export class EntryCardComponent {
  entry = input.required<Entry>();
}
