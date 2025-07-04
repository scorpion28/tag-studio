import { Component, inject, signal, ElementRef, ViewChild, HostListener, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { debounceTime, distinctUntilChanged, switchMap, tap, of } from 'rxjs';
import { SearchService } from '../../../services/search.service';
import { SearchResultItem } from '../../../models/search-result-item';

@Component({
  selector: 'app-search-bar',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './search-bar.component.html',
})
export class SearchBarComponent implements OnInit {
  private searchService = inject(SearchService);
  private router = inject(Router);
  
  searchControl = new FormControl('');
  searchResults = signal<SearchResultItem[]>([]);
  searchHistory = signal<string[]>([]);
  showResults = signal(false);
  isLoading = signal(false);

  @ViewChild('searchInput') searchInput!: ElementRef;

  ngOnInit() {
    this.searchHistory.set(this.searchService.getHistory());

    this.searchControl.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      tap(() => this.isLoading.set(true)),
      switchMap(query => {
        if (!query || query.trim().length < 2) {
          this.isLoading.set(false);
          return of([]);
        }
        return this.searchService.search({
          query: query,
          pageNumber: 1,
          pageSize: 10
        });
      }),
      tap(results => {
        this.searchResults.set(results);
        this.isLoading.set(false);
        this.showResults.set(true);
      })
    ).subscribe();
  }

  onFocus() {
    this.searchHistory.set(this.searchService.getHistory());
    this.showResults.set(true);
  }

  onBlur() {
    // Delay hiding results so that clicks on results can register
    setTimeout(() => this.showResults.set(false), 200);
  }

  selectResult(result: SearchResultItem) {
    this.searchService.addHistory(this.searchControl.value || "");
    this.showResults.set(false);
    this.searchControl.setValue('', { emitEvent: false });
    
    if (result.type === 'Tag') {
      this.router.navigate(['/app/tags'], { queryParams: { selected: result.id } });
    } else {
      this.router.navigate(['/app/entries'], { queryParams: { selected: result.id } });
    }
  }

  selectHistory(query: string) {
    this.searchControl.setValue(query);
    this.searchInput.nativeElement.focus();
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    if (this.searchInput && !this.searchInput.nativeElement.contains(event.target)) {
      this.showResults.set(false);
    }
  }
}
