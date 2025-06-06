import { computed, inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  BehaviorSubject,
  catchError, combineLatest,
  concatMap, debounceTime, distinctUntilChanged, EMPTY,
  map,
  merge,
  mergeMap,
  Observable, skip, startWith,
  Subject,
  switchMap,
} from 'rxjs';
import { PaginatedList } from '../../../shared/models/paginated-list';
import { TagDetailed } from '../models/tag-detailed.model';
import { CreateTag, EditTag, RemoveTag, Tag, toTagModel } from '../models/tag.model';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { PaginationInfo } from '../../../shared/models/pagination';

interface TagsState {
  tagsPaginated: PaginatedList<Tag>;
  loading: boolean,
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
    loading: false,
    error: null,
  });

  tags = computed(() => this.state().tagsPaginated.items);
  pagination = computed(() => this.state().tagsPaginated as PaginationInfo);

  add$ = new Subject<CreateTag>();
  edit$ = new Subject<EditTag>();
  remove$ = new Subject<RemoveTag>();
  page$ = new BehaviorSubject<number>(1);
  pageSize$ = new BehaviorSubject<number>(10);

  private tagAdded$ = this.add$.pipe(
    mergeMap(addTag =>
      this.http
        .post(this.baseUrl, {
          name: addTag.name,
          parentIds: addTag.parents.map(tag => tag.id),
        })
        .pipe(catchError(err => this.handleError(err))),
    ),
  );

  private tagEdited$ = this.edit$.pipe(
    concatMap(update =>
      this.http
        .patch(`${this.baseUrl}/${update.id}`, {
          name: update.data.name,
          parentTagsIds: update.data.parents.map(tag => tag.id),
        })
        .pipe(catchError(err => this.handleError(err)))),
  );

  private tagRemoved$ = this.remove$.pipe(
    mergeMap((id) =>
      this.http
        .delete(`${this.baseUrl}/${id}`)
        .pipe(catchError(err => this.handleError(err))),
    ),
  );

  private paginationUpdated$ = combineLatest([this.page$, this.pageSize$])
    .pipe(
      map(([pageNumber, pageSize]) =>
        ({ page: pageNumber, pageSize: pageSize }),
      ),
      debounceTime(300),
      distinctUntilChanged((prev, curr) =>
        prev.page === curr.page && prev.pageSize == curr.pageSize),
    );

  private tagsLoaded$ = merge(this.tagAdded$, this.tagEdited$, this.tagRemoved$)
    .pipe(
      startWith(null),
      switchMap(() => this.paginationUpdated$),
      switchMap(({ page, pageSize }) =>
        this.getTags(page, pageSize).pipe(
          catchError(err => this.handleError(err)),
        ),
      ),
    );

  constructor() {
    this.tagsLoaded$
      .pipe(takeUntilDestroyed())
      .subscribe((tagsPaginated) =>
        this.state.update(state => ({
            ...state,
            tagsPaginated: tagsPaginated,
            loading: false,
          }),
        ),
      );

    // Reset the page to 1 when pageSize changes
    this.pageSize$
      .pipe(
        takeUntilDestroyed(),
        skip(1),
      )
      .subscribe(() => this.page$.next(1));
  }

  getTags(pageNumber: number = 1, pageSize: number = 5): Observable<PaginatedList<Tag>> {
    this.state.update((state) => ({ ...state, loaded: true }));

    return this.http.get<PaginatedList<TagDetailed>>(this.baseUrl, {
      params: {
        pageNumber: Math.floor(pageNumber),
        pageSize: Math.floor(pageSize),
      },
    })
      .pipe(
        map(list => ({
          ...list,
          items: list.items.map(tagDto => toTagModel(tagDto)),
        })),
      );
  }

  getTagById(id: string): Observable<Tag> {
    return this.http.get<TagDetailed>(`${this.baseUrl}/${id}`)
      .pipe(
        map(tagDto => toTagModel(tagDto)),
      );
  }

  private handleError(err: any) {
    this.state.update(state => ({
        ...state,
        error: err,
      }),
    );

    console.error(err);
    return EMPTY;
  }
}
