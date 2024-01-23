import { apiSlice } from "../../app/api/apiSlice";

export const catalogApiSlice = apiSlice.injectEndpoints({
    endpoints: builder => ({
        getCatalog: builder.query({
            query: () => '/api/catalog/products'
        }),
        getProductById: builder.query({
            query: (productId) => `/api/catalog/products/id/${productId}`
        }),
        getStoreCatalog: builder.query({
            query: (storeId) => `/api/catalog/products/store/${storeId}`
        }),
        addToBasket: builder.mutation({
            query: productinfo => ({
                url: '/api/catalog/products/updatebasket',
                method: 'POST',
                body: { ...productinfo }
            })
        })
    })
})

export const {
    useGetCatalogQuery,
    useGetProductByIdQuery,
    useAddToBasketMutation
} = catalogApiSlice