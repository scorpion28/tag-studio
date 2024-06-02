import { Component, computed, inject, input, linkedSignal, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EMPTY } from 'rxjs';
import { EntryService } from '../services/entry.service';
import { TagBrief } from '../../tag/models/tag-brief.model';
import { TagPickerComponent } from '../../tag/tag-picker/tag-picker.component';
import { ModalWrapperComponent } from '../../../shared/components/modal-wrapper/modal-wrapper';
import { rxResource } from '@angular/core/rxjs-interop';
import { TagListComponent } from '../../tag/tag-list.component';
import { CreateEntry, EditEntry } from '../models/entry.model';

@Component({
  selector: 'app-entry-form',
  standalone: true,
  imports: [
    FormsModule,
    TagPickerComponent,
    ModalWrapperComponent,
    TagListComponent,
  ],
  templateUrl: './entry-form.component.html',
})
export class EntryFormComponent {
  entryId = input<string>('');
  close = output<CreateEntry | EditEntry | null>();

  entryService = inject(EntryService);

  isEditMode = computed(() => !!this.entryId());
  tagPickerVisible = signal(false);

  entryLoaded = rxResource({
    params: () => ({
      isEdit: this.isEditMode(),
      entryId: this.entryId(),
    }),
    stream: ({ params }) => {
      if (!params.isEdit) return EMPTY;

      return this.entryService.getEntryById(params.entryId);
    },
  });

  entry = computed(() => this.isEditMode() ? this.entryLoaded.value() : undefined);

  name = linkedSignal(() => this.entry()?.name ?? '');
  description = linkedSignal(() => this.entry()?.description ?? undefined);
  tags = linkedSignal(() => this.entry()?.tags ?? []);

  onFormClose() {
    this.close.emit(this.formatData());
  }

  formatData() {
    const isEdit = this.isEditMode();
    const initialData = this.entry();
    const updated = {
      name: this.name(),
      description: this.description(),
      parentTags: this.tags(),
    };

    if (isEdit) {
      if (!initialData?.id) {
        return null;
      }

      return {
        id: initialData.id,
        data: {
          name: updated.name,
          description: updated.description,
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

  removeEntryTag(tag: TagBrief) {
    this.entryService.removeTagFromEntry(this.entry()?.id!, tag.id)
      .subscribe(_ => this.tags.update(tags => tags.filter(t => t.id !== tag.id)));
  }

  addEntryTag(tag: TagBrief) {
    if (this.isEditMode()) {
      this.entryService.addTagsToEntry(this.entry()?.id!, [tag.id])
        .subscribe(_ => this.tags.update(tags => [...tags, tag]));
    } else {
      this.tags.update((tags) => [...tags, tag]);
    }
  }
}
