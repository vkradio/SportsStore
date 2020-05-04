export class Filter {
  category?: string;
  search?: string;
  related = false;

  reset() {
    this.category = this.search = null;
    this.related = false;
  }
}

export class Pagination {
  productsPerPage = 4;
  currentPage = 1;
}
