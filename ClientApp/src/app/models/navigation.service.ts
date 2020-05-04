import { Injectable } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { Repository } from './repository';
import { filter } from 'rxjs/operators';
import { element } from 'protractor';

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
      if (active.params.categoryOrPage !== undefined) {
        const value = Number.parseInt(active.params.categoryOrPage, 10);
        if (!Number.isNaN(value)) {
          this.repo.filter.category = '';
          this.repo.paginationObject.currentPage = value;
        } else {
          this.repo.filter.category = active.params.categoryOrPage;
          this.repo.paginationObject.currentPage = 1;
        }
      } else {
        const category = active.params.category;
        this.repo.filter.category = category || '';
        this.repo.paginationObject.currentPage = Number.parseInt(active.params.page, 10) || 1;
      }
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

  get currentPage() {
    return this.repo.paginationObject.currentPage;
  }

  set currentPage(newPage: number) {
    if (this.currentCategory === '') {
      this.router.navigateByUrl(`/store/${newPage}`);
    } else {
      this.router.navigateByUrl(`/store/${this.currentCategory}/${newPage}`);
    }
  }

  get productsPerPage() {
    return this.repo.paginationObject.productsPerPage;
  }

  get productCount() {
    return (this.repo.products || []).length;
  }
}
