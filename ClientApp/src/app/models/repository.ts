import { Product } from './product.model';
import { Supplier } from './supplier.model';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Filter, Pagination } from './configClasses.repository';
import { Observable } from 'rxjs';
import { Order, OrderConfirmation } from './order.model';

const productsUrl = '/api/products';
const suppliersUrl = '/api/suppliers';
const sessionUrl = '/api/session';
const ordersUrl = '/api/orders';
const loginUrl = '/api/account/';

interface ProductsMetadata {
  data: Product[];
  categories: string[];
}

@Injectable()
export class Repository {
  product: Product;
  products: Product[];
  suppliers: Supplier[] = [];
  filter = new Filter();
  categories: string[] = [];
  paginationObject = new Pagination();
  orders: Order[] = [];

  constructor(private http: HttpClient) {
    this.filter.related = true;
  }

  getProduct(id: number) {
    this
      .http
      .get<Product>(`${productsUrl}/${id}`)
      .subscribe(p => this.product = p);
    console.log('request');
  }

  async getProducts() {
    let url = `${productsUrl}?related=${this.filter.related}`;
    if (this.filter.category) {
      url += `&category=${this.filter.category}`;
    }
    if (this.filter.search) {
      url += `&search=${this.filter.search}`;
    }
    url += '&metadata=true';

    const md = await this
      .http
      .get<ProductsMetadata>(url)
      .toPromise<ProductsMetadata>();
    this.products = md.data;
    this.categories = md.categories;
    return md;
  }

  getSuppliers() {
    this
      .http
      .get<Supplier[]>(suppliersUrl)
      .subscribe(sups => this.suppliers = sups);
  }

  createProduct(prod: Product) {
    const data = {
      name: prod.name,
      category: prod.category,
      description: prod.description,
      price: prod.price,
      supplier: prod.supplier ? prod.supplier.supplierId : 0
    };

    this
      .http
      .post<number>(productsUrl, data)
      .subscribe(id => {
        prod.productId = id;
        this.products.push(prod);
      });
  }

  createProductAndSupplier(prod: Product, supp: Supplier) {
    const data = {
      name: supp.name,
      city: supp.city,
      state: supp.state
    };

    this
      .http
      .post<number>(suppliersUrl, data)
      .subscribe(id => {
        supp.supplierId = id;
        prod.supplier = supp;
        this.suppliers.push(supp);
        if (prod != null) {
          this.createProduct(prod);
        }
      });
  }

  replaceProduct(prod: Product) {
    const data = {
      name: prod.name,
      category: prod.category,
      description: prod.description,
      price: prod.price,
      supplier: prod.supplier ? prod.supplier.supplierId : 0
    };

    this
      .http
      .put(`${productsUrl}/${[prod.productId]}`, data)
      .subscribe(() => this.getProducts());
  }

  replaceSupplier(supp: Supplier) {
    const data = {
      name: supp.name,
      city: supp.city,
      state: supp.state
    };

    this
      .http
      .put(`${suppliersUrl}/${supp.supplierId}`, data)
      .subscribe(() => this.getProducts());
  }

  updateProduct(id: number, changes: Map<string, any>) {
    const patch = [];

    changes
      .forEach((val, key) => patch.push({ op: 'replace', path: key, value: val }));

    this
      .http
      .patch(`${productsUrl}/${id}`, patch)
      .subscribe(() => this.getProducts());
  }

  deleteProduct(id: number) {
    this
      .http
      .delete(`${productsUrl}/${id}`)
      .subscribe(() => this.getProducts());
  }

  deleteSupplier(id: number) {
    this
      .http
      .delete(`${suppliersUrl}/${id}`)
      .subscribe(() => {
        this.getProducts();
        this.getSuppliers();
      });
  }

  storeSessionData<T>(dataType: string, data: T) {
    return this
      .http
      .post(`${sessionUrl}/${dataType}`, data)
      .subscribe(response => { });
  }

  getSessionData<T>(dataType: string): Observable<T> {
    return this
      .http
      .get<T>(`${sessionUrl}/${dataType}`);
  }

  getOrders() {
    this
      .http
      .get<Order[]>(ordersUrl)
      .subscribe(data => this.orders = data);
  }

  createOrder(order: Order) {
    this
      .http
      .post<OrderConfirmation>(ordersUrl, {
        name: order.name,
        address: order.address,
        payment: order.payment,
        products: order.products
      })
      .subscribe(data => {
        order.orderConfirmation = data;
        order.cart.clear();
        order.clear();
      });
  }

  shipOrder(order: Order) {
    this
      .http
      .post(`${ordersUrl}/${order.orderId}`, {})
      .subscribe(() => this.getOrders());
  }

  login(name: string, password: string) {
    return this
      .http
      .post<boolean>(`${loginUrl}login`, { name, password });
  }

  logout() {
    this
      .http
      .post(`${loginUrl}logout`, null)
      .subscribe(response => { });
  }
}
