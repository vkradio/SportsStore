import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Repository } from '../models/repository';
import { Product } from '../models/product.model';

@Component({
  selector: 'app-product-detail',
  templateUrl: 'productDetail.component.html'
})
export class ProductDetailComponent {

  constructor(
    private repo: Repository,
    router: Router,
    activeRoute: ActivatedRoute) {

      const id = Number.parseInt(activeRoute.snapshot.params.id, 10);
      if (id) {
        this.repo.getProduct(id);
      } else {
        router.navigateByUrl('/');
      }
    }

  get product() {
    return this.repo.product;
  }
}
