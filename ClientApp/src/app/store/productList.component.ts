import { Component } from '@angular/core';
import { Repository } from '../models/repository';
import { Product } from '../models/product.model';
import { Cart } from '../models/cart.model';

@Component({
  selector: 'app-store-product-list',
  templateUrl: 'productList.component.html'
})
export class ProductListComponent {
  constructor(private repo: Repository, private cart: Cart) { }

  get products() {
    if (this.repo.products != null && this.repo.products.length > 0) {
      const p = this.repo.paginationObject;
      const pageIndex = (p.currentPage - 1) * p.productsPerPage;
      return this
        .repo
        .products
        .slice(pageIndex, pageIndex + p.productsPerPage);
    }
  }

  addToCart(product: Product) {
    this.cart.addProduct(product);
  }
}
