import { Injectable, NgZone } from '@angular/core';
import { Repository } from './models/repository';
import { Product } from './models/product.model';
import { NavigationService } from './models/navigation.service';

interface DotnetInvokable {
  invokeMethod<T>(methodName: string, ...args: any): T;
  invokeMethodAsync<T>(methodName: string, ...args: any): Promise<T>;
}

const globalFuncSearch = 'angular_searchProducts';
const globalFuncReceiveReference = 'angular_receiveReference';

@Injectable()
export class ExternalService {

  private resetFunction: (msg: string) => { };

  constructor(
    private repo: Repository,
    private zone: NgZone,
    navService: NavigationService) {

    window[globalFuncSearch] = this.doSearch.bind(this);
    window[globalFuncReceiveReference] = this.receiveReference.bind(this);

    navService.change.subscribe(() => {
      if (this.resetFunction) {
        this.resetFunction('Results reset');
      }
    });
  }

  async doSearch(searchTerm: string) {
    return this.zone.run(async () => {
      this.repo.filter.search = searchTerm;
      return (await this.repo.getProducts()).data;
    });
  }

  receiveReference(target: DotnetInvokable) {
    this.resetFunction = (msg: string) => target.invokeMethod('ResetSearch', msg);
  }
}
