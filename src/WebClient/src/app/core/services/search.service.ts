import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { SearchRequest, SearchResultItem } from '../models/search-result-item';

@Injectable({
  providedIn: 'root'
})
export class SearchService {
  private http = inject(HttpClient);
  private readonly historyKey = 'tag-studio-search-history';
  private readonly maxHistory = 5;

  search(req: SearchRequest): Observable<SearchResultItem[]> {
    let params = new HttpParams()
      .set('pageNumber', req.pageNumber.toString())
      .set('pageSize', req.pageSize.toString());

    if (req.query) params = params.set('query', req.query);
    if (req.fromDate) params = params.set('fromDate', req.fromDate);
    if (req.toDate) params = params.set('toDate', req.toDate);
    if (req.extension) params = params.set('extension', req.extension);

    return this.http.get<SearchResultItem[]>('/api/search', { params });
  }

  getHistory(): string[] {
    const history = localStorage.getItem(this.historyKey);
    return history ? JSON.parse(history) : [];
  }

  addHistory(query: string): void {
    if (!query || query.trim() === '') return;
    
    let history = this.getHistory();
    history = history.filter(h => h.toLowerCase() !== query.toLowerCase());
    history.unshift(query);
    
    if (history.length > this.maxHistory) {
      history = history.slice(0, this.maxHistory);
    }
    
    localStorage.setItem(this.historyKey, JSON.stringify(history));
  }

  clearHistory(): void {
    localStorage.removeItem(this.historyKey);
  }
}
