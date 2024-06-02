import { Component, computed, inject, input, linkedSignal, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TagBrief } from '../models/tag-brief.model';
import { EMPTY } from 'rxjs';
import { TagPickerComponent } from '../tag-picker/tag-picker.component';
import { rxResource } from '@angular/core/rxjs-interop';
import { TagService } from '../services/tag.service';
import { ModalWrapperComponent } from '../../../shared/components/modal-wrapper/modal-wrapper';
import { TagListComponent } from '../tag-list.component';
import { CreateTag, EditTag } from '../models/tag.model';

@Component({
  selector: 'app-tag-form',
  standalone: true,
  imports: [
    FormsModule,
    TagPickerComponent,
    ModalWrapperComponent,
    TagListComponent,
  ],
  templateUrl: './tag-form.component.html',
})
export class TagFormComponent {
  tagId = input<string>('');
  close = output<CreateTag | EditTag | null>();

  tagService = inject(TagService);

  isEditMode = computed(() => !!this.tagId());
  tagPickerVisible = signal(false);

  tagLoaded = rxResource({
    params: () => ({
      isEdit: this.isEditMode(),
      tagId: this.tagId(),
    }),
    stream: ({ params }) => {
      if (!params.isEdit) return EMPTY;

      return this.tagService.getTagById(params.tagId);
    },
  });

  tag = computed(() => {
    if (this.isEditMode()) {
      return this.tagLoaded.value();
    }
    return undefined;
  });

  // Form elements
  tagName = linkedSignal(() => this.tag()?.name ?? '');
  parentTags = linkedSignal(() => this.tag()?.parents ?? []);

  onFormClose() {
    this.close.emit(this.formatData());
  }

  formatData(): CreateTag | EditTag | null {
    const isEdit = this.isEditMode();
    const initialData = this.tag();
    const updated = {
      name: this.tagName(),
      parentTags: this.parentTags(),
    };

    if (isEdit) {
      if (!initialData?.id) {
        return null;
      }

      return {
        id: initialData.id,
        data: {
          name: updated.name,
          parents: updated.parentTags,
        },
      };
    }

    if (updated.name.trim().length === 0) {
      return null;
    }

    return {
      name: updated.name,
      parents: updated.parentTags,
    };
  }

  onParentTagAdded(tag: TagBrief) {
    this.parentTags.update(tags => [...tags, tag]);
  }

  onParentTagRemoved(tag: TagBrief) {
    this.parentTags.update(tags => tags.filter(value => value !== tag));
  }
}
