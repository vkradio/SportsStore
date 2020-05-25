import { Injectable } from '@angular/core';
import { Repository } from './models/repository';
import { Product } from './models/product.model';

const globalFuncSearch = 'angular_searchProducts';

@Injectable()
export class ExternalService {
  constructor(private repo: Repository) {
    window[globalFuncSearch] = this.doSearch.bind(this);
  }

  doSearch(searchTerm: string) {
    const lowerTerm = searchTerm.toLowerCase();
    return this
      .repo
      .products
      .filter(p => p.name.toLowerCase().includes(lowerTerm) ||
                   p.description.toLowerCase().includes(lowerTerm));
  }
}
