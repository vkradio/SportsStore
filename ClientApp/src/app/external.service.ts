import { Injectable, NgZone } from '@angular/core';
import { Repository } from './models/repository';
import { Product } from './models/product.model';

const globalFuncSearch = 'angular_searchProducts';

@Injectable()
export class ExternalService {
  constructor(private repo: Repository, private zone: NgZone) {
    window[globalFuncSearch] = this.doSearch.bind(this);
  }

  async doSearch(searchTerm: string) {
    return this.zone.run(async () => {
      this.repo.filter.search = searchTerm;
      return (await this.repo.getProducts()).data;
    });
  }
}
