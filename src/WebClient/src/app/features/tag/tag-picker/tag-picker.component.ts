import { Component, computed, inject, input, output } from '@angular/core';
import { TagBrief } from '../models/tag-brief.model';
import { TagService } from '../services/tag.service';
import { map } from 'rxjs';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-tag-picker',
  standalone: true,
  imports: [],
  styleUrl: "tag-picker.component.css",
  template: `
    <div class="tag-picker-modal" (click)="closePicker()">
      <div class="picker-content text-white bg-neutral-600 rounded-md" (click)="$event.stopPropagation()"
           style="background-color: rgb(37, 37, 37); max-width: 672px;">
        <div class="flex flex-wrap items-center bg-neutral-700 p-2 rounded-md gap-1.5"
             style="background-color: rgba(255, 255, 255, 0.03);">
          @for (tag of selectedTags(); track tag.id) {
            <div class="flex items-center bg-green-900 px-1 rounded-xs">
              <span class="whitespace-nowrap">{{ tag.name }}</span>
              <button type="button" (click)="onTagUnselected(tag)"
                      class="ml-1 text-gray-200 hover:text-white focus:outline-none focus:ring-2 focus:ring-white focus:ring-opacity-50">
                <svg class="h-3 w-3 hover:bg-neutral-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                </svg>
              </button>
            </div>
          }
          <input class="outline-0" type="text">
        </div>
        <div class="p-2">
          <p class="text-xs font-medium px-1" style="color: rgba(255, 255, 255, 0.46)">Select an option or create
            one</p>
          <div>
            @for (tag of availableTags(); track tag.id) {
              <div class=" hover:bg-neutral-700 p-1 rounded-md" (click)="onTagSelected(tag)">
                <span class="bg-green-900 px-1.5 py-0.5 rounded-sm mt-5">{{ tag.name }}</span>
              </div>
            }
          </div>
        </div>
      </div>
    </div>
  `
})
export class TagPickerComponent {
  selectedTags = input.required<TagBrief[]>();

  tagAdded = output<TagBrief>();
  tagRemoved = output<TagBrief>();

  close = output();

  private tagService = inject(TagService);

  closePicker() {
    this.close.emit();
  }

  private readonly allTags$ = this.tagService.getTags().pipe(
    map(list => list.items)
  );
  private readonly allTags = toSignal(this.allTags$, { initialValue: [] });

  readonly availableTags = computed(() => {
    const all = this.allTags();
    const selected = this.selectedTags();

    if (!all || all.length === 0) {
      return [];
    }

    return all.filter(
      (tag) => !selected.some((selectedTag) => selectedTag.id === tag.id)
    );
  });

  onTagSelected(tag: TagBrief) {
    this.tagAdded.emit(tag);
  }

  onTagUnselected(tag: TagBrief) {
    this.tagRemoved.emit(tag);
  }
}
