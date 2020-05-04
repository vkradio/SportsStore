import { Injectable } from '@angular/core';
import { Product } from './product.model';

@Injectable()
export class Cart {
  selections: ProductSelection[] = [];
  itemCount = 0;
  totalPrice = 0;

  addProduct(product: Product) {
    const selection = this
      .selections
      .find(ps => ps.productId === product.productId);

    if (selection) {
      selection.quantity++;
    } else {
      this
        .selections
        .push(new ProductSelection(
          this,
          product.productId,
          product.name,
          product.price,
          1
        ));
    }

    this.update();
  }

  updateQuantity(productId: number, quantity: number) {
    if (quantity > 0) {
      const selection = this
        .selections
        .find(ps => ps.productId === productId);

      if (selection) {
        selection.quantity = quantity;
      }
    } else {
      const index = this
        .selections
        .findIndex(ps => ps.productId === productId);

      if (index !== -1) {
        this.selections.splice(index, 1);
      }

      this.update();
    }
  }

  clear() {
    this.selections = [];
    this.update();
  }

  update() {
    this.itemCount = this
      .selections
      .map(ps => ps.quantity)
      .reduce((prev, curr) => prev + curr, 0);
    this.totalPrice = this
      .selections
      .map(ps => ps.price * ps.quantity)
      .reduce((prev, curr) => prev + curr, 0);
  }
}

export class ProductSelection {
  constructor(
    public cart: Cart,
    public productId?: number,
    public name?: string,
    public price?: number,
    private quantityValue?: number
  ) { }

  get quantity() {
    return this.quantityValue;
  }

  set quantity(newQuantity) {
    this.quantityValue = newQuantity;
    this.cart.update();
  }
}
