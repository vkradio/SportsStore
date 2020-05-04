import { Injectable } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { Repository } from './repository';
import { filter } from 'rxjs/operators';

@Injectable()
export class NavigationService {
  constructor(
    private repo: Repository,
    private router: Router,
    private active: ActivatedRoute) {
      router
        .events
        .pipe(filter(event => event instanceof NavigationEnd))
        .subscribe(ev => this.handleNavigationChange());
    }

  private handleNavigationChange() {
    const active = this.active.firstChild.snapshot;
    if (active.url.length > 0 && active.url[0].path === 'store') {
      const category = active.params.category;
      this.repo.filter.category = category || '';
      this.repo.getProducts();
    }
  }

  get categories() {
    return this.repo.categories;
  }

  get currentCategory() {
    return this.repo.filter.category || '';
  }

  set currentCategory(newCategory: string) {
    this.router.navigateByUrl(`/store/${(newCategory || '').toLowerCase()}`);
  }
}
