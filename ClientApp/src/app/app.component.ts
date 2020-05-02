import { Component } from '@angular/core';
import { Repository } from './models/repository';
import { Product } from './models/product.model';
import { Supplier } from './models/supplier.model';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  constructor(private repo: Repository) { }

  get product(): Product {
    return this.repo.product;
  }

  get products(): Product[] {
    return this.repo.products;
  }

  createProduct() {
    this.repo.createProduct(new Product(
      0,
      'X-Ray Scuba Mask',
      'Watersports',
      'See what the fish are hiding',
      49.99,
      this.repo.products[0].supplier
    ));
  }

  createProductAndSupplier() {
    const supplier = new Supplier(0, 'Rocket Shoe Corp', 'Boston', 'MA');
    const product = new Product(
      0,
      'Rocket-Powered Shoes',
      'Running',
      'Set a new record',
      100,
      supplier
    );
    this.repo.createProductAndSupplier(product, supplier);
  }

  replaceProduct() {
    const product = this.repo.products[0];
    product.name = 'Modified Product';
    product.category = 'Modified Category';
    this.repo.replaceProduct(product);
  }

  replaceSupplier() {
    const supplier = new Supplier(3, 'Modified Supplier', 'New York', 'NY');
    this.repo.replaceSupplier(supplier);
  }

  updateProduct() {
    const changes = new Map<string, any>();
    changes.set('name', 'Green Kayak');
    changes.set('supplier', null);
    this.repo.updateProduct(1, changes);
  }
}
