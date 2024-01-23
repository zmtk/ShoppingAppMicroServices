import { apiSlice } from "../../app/api/apiSlice";

export const storeApiSlice = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    getStores: builder.query({
      query: () => '/api/catalog/store'
    }),
    getProductsByStoreId: builder.query({
      query: (storeId) => `/api/catalog/products/store/${storeId}`
    }),
    addStore: builder.mutation({
      query: storeData => ({
        url: '/api/catalog/store',
        method: 'POST',
        body: { ...storeData }
      }),
    }),
    deleteStore: builder.mutation({
      query: (storeId) => ({
        url: `/api/catalog/store/${storeId}`,
        method: 'DELETE',
      }),
    }),
    addProduct: builder.mutation({
      query: productData => ({
        url: '/api/catalog/products/',
        method: 'POST',
        body: { ...productData }
      }),
    }),
    updateProduct: builder.mutation({
      query: productData => ({
        url: '/api/catalog/products/updateprice',
        method: 'POST',
        body: { ...productData }
      }),
    }),
    toggleStoreProduct: builder.mutation({
      query: (productId) => ({
        url: `/api/catalog/products/${productId}`,
        method: 'PUT',
      }),
    })
  })
});

export const {
  useGetStoresQuery,
  useGetProductsByStoreIdQuery,
  useAddStoreMutation,
  useDeleteStoreMutation,
  useAddProductMutation,
  useUpdateProductMutation,
  useToggleStoreProductMutation,
} = storeApiSlice;