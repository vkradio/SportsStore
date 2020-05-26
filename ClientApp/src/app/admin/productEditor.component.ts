import { Component } from '@angular/core';
import { Repository } from '../models/repository';
import { Product } from '../models/product.model';
import { Supplier } from '../models/supplier.model';

@Component({
  selector: 'app-admin-product-editor',
  templateUrl: 'productEditor.component.html'
})
export class ProductEditorComponent {
  constructor(private repo: Repository) { }

  get product() {
    return this.repo.product;
  }

  get suppliers() {
    return this.repo.suppliers;
  }

  compareSuppliers(s1: Supplier, s2: Supplier) {
    return s1 && s2 && s1.name === s2.name;
  }
}
