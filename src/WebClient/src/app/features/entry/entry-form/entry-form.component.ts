import { Component, computed, inject, input, linkedSignal, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EMPTY } from 'rxjs';
import { EntryService } from '../services/entry.service';
import { TagBrief } from '../../tag/models/tag-brief.model';
import { TagPickerComponent } from '../../tag/tag-picker/tag-picker.component';
import { rxResource } from '@angular/core/rxjs-interop';
import { TagListComponent } from '../../tag/tag-list.component';
import { CreateEntry, EditEntry } from '../models/entry.model';
import { HttpClient } from '@angular/common/http';
import { ImagePickerComponent } from './ui/image-picker.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-entry-form',
  standalone: true,
  imports: [
    FormsModule,
    TagPickerComponent,
    TagListComponent,
    ImagePickerComponent,
  ],
  templateUrl: './entry-form.component.html',
})
export class EntryFormComponent {
  entryId = input<string>('');
  save = output<CreateEntry | EditEntry | null>();

  private entryService = inject(EntryService);
  private http = inject(HttpClient);
  private router = inject(Router);

  isEditMode = computed(() => !!this.entryId());
  tagPickerVisible = signal(false);
  imagePickerVisible = signal(false);

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

  onClose() {
    this.save.emit(this.formatData());
  }

  onImageSelected(event: Event): void {
    this.imagePickerVisible.set(false);
    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length > 0) {
      const image = input.files[0];
      this.sendImage(image);
    }
  }

  onImageRemove() {
    this.imagePickerVisible.set(false);

    this.removeImage();
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

  sendImage(image: File): void {
    if (!image) return;

    if (this.isEditMode()) {
      this.uploadImage(this.entryId(), image);
    } else {
      const data = this.formatData() as CreateEntry;
      if (!data) return;

      this.entryService.createEntry(data).subscribe(newEntry => {
        this.router.navigate([], {
          queryParams: { selected: newEntry.id },
          queryParamsHandling: 'merge',
        });
        this.uploadImage(newEntry.id, image);
      });
    }
  }

  private uploadImage(entryId: string, image: File): void {
    const formData = new FormData();
    formData.append('file', image);

    this.http.post(`/api/entries/${entryId}/image`, formData)
      .subscribe(() => this.entryLoaded.reload());
  }

  removeImage() {
    if (this.isEditMode() && this.entry()?.imageUrl) {
      this.http.delete(`/api/entries/${this.entryId()}/image`)
        .subscribe(() => this.entryLoaded.reload());
    }
  }
}
