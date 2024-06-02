import { Component, input } from '@angular/core';
import { TagBrief } from './models/tag-brief.model';

@Component({
  selector: 'app-tag-list',
  template: `
    <div class="text-alpha-87 flex gap-1.5" [class.flex-wrap]="wrap()">
      @for (tag of tags(); track tag.id) {
        <span class="bg-green-900  px-1.5  py-0.5 rounded-sm  whitespace-nowrap">{{ tag.name }}</span>
      }
    </div>
  `,
})
export class TagListComponent {
  wrap = input<boolean>(false);
  tags = input.required<TagBrief[]>();
}
