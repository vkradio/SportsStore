import { Component } from '@angular/core';
import { Repository } from '../models/repository';
import { Product } from '../models/product.model';
import { Supplier } from '../models/supplier.model';

@Component({
  templateUrl: 'productAdmin.component.html'
})
export class ProductAdminComponent {
  constructor(private repo: Repository) { }

  tableMode = true;

  get product() {
    return this.repo.product;
  }

  selectProduct(id: number) {
    this.repo.getProduct(id);
  }

  saveProduct() {
    if (this.repo.product.productId == null) {
      this.repo.createProduct(this.repo.product);
    } else {
      this.repo.replaceProduct(this.repo.product);
    }
    this.clearProduct();
  }

  deleteProduct(id: number) {
    this.repo.deleteProduct(id);
  }

  clearProduct() {
    this.repo.product = new Product();
    this.tableMode = true;
  }

  get products() {
    return this.repo.products;
  }
}
