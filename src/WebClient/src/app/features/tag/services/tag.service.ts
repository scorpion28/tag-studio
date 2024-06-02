import { computed, inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  catchError,
  concatMap,
  map,
  merge,
  mergeMap,
  Observable,
  of,
  startWith,
  Subject,
  switchMap,
} from 'rxjs';
import { PaginatedList } from '../../../shared/models/paginated-list';
import { TagDetailed } from '../models/tag-detailed.model';
import { CreateTag, EditTag, RemoveTag, Tag, toTagModel } from '../models/tag.model';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

interface TagsState {
  tagsPaginated: PaginatedList<Tag>;
  loaded: boolean,
  error: any
}

@Injectable({
  providedIn: 'root',
})
export class TagService {
  private readonly baseUrl = 'api/tags';

  private http = inject(HttpClient);

  private state = signal<TagsState>({
    tagsPaginated: {
      items: [],
      pageNumber: 1,
      hasPreviousPage: false,
      hasNextPage: false,
      totalCount: 0,
      totalPages: 1,
    },
    loaded: false,
    error: null,
  });

  tags = computed(() => this.state().tagsPaginated.items);

  add$ = new Subject<CreateTag>();
  edit$ = new Subject<EditTag>();
  remove$ = new Subject<RemoveTag>();

  tagAdded$ = this.add$.pipe(
    concatMap(addTag =>
      this.http
        .post(this.baseUrl, {
          name: addTag.name,
          parentIds: addTag.parents.map(tag => tag.id),
        })
        .pipe(catchError(err => this.handleError(err))),
    ),
  );

  tagEdited$ = this.edit$.pipe(
    mergeMap(update =>
      this.http
        .patch(`${this.baseUrl}/${update.id}`, {
          name: update.data.name,
          parentTagsIds: update.data.parents.map(tag => tag.id),
        })
        .pipe(catchError(err => this.handleError(err)))),
  );

  tagRemoved$ = this.remove$.pipe(
    mergeMap((id) =>
      this.http
        .delete(`${this.baseUrl}/${id}`)
        .pipe(catchError(err => this.handleError(err))),
    ),
  );

  getTags(pageNumber: number = 1, pageSize: number = 20): Observable<PaginatedList<Tag>> {
    return this.http.get<PaginatedList<TagDetailed>>(this.baseUrl, {
      params: {
        pageNumber: Math.floor(pageNumber),
        pageSize: Math.floor(pageSize),
      },
    })
      .pipe(
        map(list => {
          return {
            ...list,
            items: list.items.map(tagDto => toTagModel(tagDto)),
          };
        }),
      );
  }

  getTagById(id: string): Observable<Tag> {
    return this.http.get<TagDetailed>(`${this.baseUrl}/${id}`)
      .pipe(
        map(tagDto => toTagModel(tagDto)),
      );
  }

  constructor() {
    merge(this.tagAdded$, this.tagEdited$, this.tagRemoved$)
      .pipe(
        startWith(null),
        switchMap(() =>
          this.getTags(),
        ),
        takeUntilDestroyed(),
      )
      .subscribe((tagsPaginated) =>
        this.state.update(state => ({
            ...state,
            tagsPaginated: tagsPaginated,
            loaded: true,
          }),
        ),
      );
  }

  private handleError(err: any) {
    return of(null);
  }
}
