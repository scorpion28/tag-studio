import { computed, inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  BehaviorSubject,
  catchError, combineLatest,
  concatMap, debounceTime, distinctUntilChanged, EMPTY,
  map,
  merge,
  mergeMap,
  Observable,
  of, skip,
  startWith,
  Subject,
  switchMap,
} from 'rxjs';
import { PaginatedList } from '../../../shared/models/paginated-list';
import { CreateEntry, EditEntry, Entry, RemoveEntry, toEntryModel } from '../models/entry.model';
import { EntryBrief } from '../models/entry-brief.model';
import { EntryDetailed } from '../models/entry-detailed.model';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { PaginationInfo } from '../../../shared/models/pagination';

interface EntriesState {
  entriesPaginated: PaginatedList<EntryBrief>;
  loading: boolean;
  error: string | null;
}

@Injectable({
  providedIn: 'root',
})
export class EntryService {
  private baseUrl = 'api/entries';

  private http = inject(HttpClient);

  private state = signal<EntriesState>({
    entriesPaginated: {
      items: [],
      pageNumber: 1,
      totalPages: 1,
      totalCount: 0,
      hasNextPage: false,
      hasPreviousPage: false,
    },
    loading: false,
    error: null,
  });

  // selectors
  entries = computed(() => this.state().entriesPaginated.items);
  pagination = computed(() => this.state().entriesPaginated as PaginationInfo);

  // sources
  add$ = new Subject<CreateEntry>();
  edit$ = new Subject<EditEntry>();
  remove$ = new Subject<RemoveEntry>();
  page$ = new BehaviorSubject<number>(1);
  pageSize$ = new BehaviorSubject<number>(10);

  private entryAdded$ = this.add$.pipe(
    concatMap(addEntry =>
      this.http
        .post(this.baseUrl, addEntry)
        .pipe(catchError(err => this.handleError(err))),
    ),
  );

  private entryEdited$ = this.edit$.pipe(
    mergeMap(update =>
      this.http
        .patch(`${this.baseUrl}/${update.id}`, update.data)
        .pipe(catchError(err => this.handleError(err))),
    ),
  );

  private entryRemoved$ = this.remove$.pipe(
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

  private entriesLoaded$ = merge(this.entryAdded$, this.entryEdited$, this.entryRemoved$)
    .pipe(
      startWith(null),
      switchMap(() => this.paginationUpdated$),
      switchMap(({ page, pageSize }) =>
        this.getEntries(page, pageSize).pipe(
          catchError(err => this.handleError(err)),
        ),
      ),
    );

  constructor() {
    // reducers
    this.entriesLoaded$
      .pipe(takeUntilDestroyed())
      .subscribe((entriesPaginated) =>
        this.state.update(state => ({
          ...state,
          entriesPaginated,
          loading: false,
        })),
      );

    // Default the page to 1 when pageSize changes
    this.pageSize$
      .pipe(
        takeUntilDestroyed(),
        skip(1),
      )
      .subscribe(() => this.page$.next(1));
  }

  private getEntries(page: number, pageSize: number): Observable<PaginatedList<EntryBrief>> {
    return this.http.get<PaginatedList<EntryBrief>>(this.baseUrl, {
      params: {
        pageNumber: Math.floor(page),
        pageSize: Math.floor(pageSize),
      },
    });
  }

  getEntryById(id: string): Observable<Entry> {
    return this.http.get<EntryDetailed>(`${this.baseUrl}/${id}`)
      .pipe(
        map(entryDto => toEntryModel(entryDto)),
      );
  }

  addTagsToEntry(entryId: string, tagIds: string[]) {
    return this.http.post(`${this.baseUrl}/${entryId}/tags`, { tagIds: tagIds })
      .pipe(
        map(_ => true),
        catchError(err => {
          console.log(err);
          return of(false);
        }),
      );
  }

  removeTagFromEntry(entryId: string, tagId: string) {
    return this.http.delete(`${this.baseUrl}/${entryId}/tags/${tagId}`);
  }

  private handleError(error: any) {
    this.state.update(state => ({
        ...state,
        error: error,
      }),
    );
    return EMPTY;
  }
}
