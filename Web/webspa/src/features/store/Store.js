import React, { useState, useEffect } from "react";

import { useGetProductsByStoreIdQuery, useGetStoresQuery } from "./storeApiSlice";
import { Product } from "./components/Product";
import StoreList from "./components/StoreList";
import { Row, Col, Card, CardBody, CardTitle, Input, Alert, FormGroup, Button } from "reactstrap";

import AddProduct from "./components/AddProduct";
import { useGetCatalogQuery } from "../catalog/catalogApiSlice";

const Store = () => {
    const { data: stores, refetch: refetchStores, isLoading, isSuccess, isError, error } = useGetStoresQuery();
    const [selectedStoreId, setSelectedStoreId] = useState('');
    const [searchTerm, setSearchTerm] = useState('');
    const [filteredProducts, setFilteredProducts] = useState([]);

    const [addProductModal, setAddProductModal] = useState(false);
    const toggleAddProductModal = () => setAddProductModal(!addProductModal);
    


    useEffect(() => {
        // Set the selected store ID to the first store when data is available
        if (isSuccess && stores.length > 0) {
            setSelectedStoreId(stores[0].id);
        }
    }, [isSuccess, stores]);

    const handleSelectStore = (storeId) => {
        setSelectedStoreId(storeId);
        localStorage.setItem("selectedStoreId", storeId);
    };

    const { data: products, refetch: refetchStoreProducts, isSuccess: productsSuccess } = useGetProductsByStoreIdQuery(selectedStoreId);
    const { refetch: refetchCatalog } = useGetCatalogQuery();

    const refetchProducts = (() => {
        refetchStoreProducts();
        refetchCatalog();
    })

    useEffect(() => {
        // Filter products based on the search term
        if (productsSuccess) {
            const filtered = products.filter(product =>
                product.name.toLowerCase().includes(searchTerm.toLowerCase())
            );
            setFilteredProducts(filtered);
        }
    }, [products, productsSuccess, searchTerm]);

    let content;

    if (isLoading) {
        content = <p>Loading...</p>;
    } else if (isSuccess && stores.length > 0) {
        content = (
            <>
                <AddProduct
                    addProductModal={addProductModal}
                    setAddProductModal={setAddProductModal}
                    refetchProducts={refetchProducts}
                    toggleAddProductModal={toggleAddProductModal}
                    selectedStoreId={selectedStoreId}
                    />


                <Card>
                    <CardTitle tag="h4" >

                        <StoreList
                            stores={stores}
                            refetchStores={refetchStores}
                            selectedStoreId={selectedStoreId}
                            onSelectStore={handleSelectStore}
                        />


                    </CardTitle>

                    {productsSuccess && (
                        <div>
                            <CardBody>
                                <FormGroup>
                                    <Row>
                                        <Col>
                                            <Input
                                                type="text"
                                                placeholder="Search products..."
                                                value={searchTerm}
                                                onChange={(e) => setSearchTerm(e.target.value)}
                                            />
                                        </Col>
                                        <Col md="4">
                                            <Button block onClick={toggleAddProductModal} color="success"> Add Product </Button>
                                        </Col>
                                    </Row>
                                </FormGroup>


                                {filteredProducts.length > 0 ? (
                                    <Row>
                                        {filteredProducts.map((product) => (
                                            <Col key={product.id} lg="3" md="6">
                                                <Product product={product} refetchProducts={refetchProducts} />
                                            </Col>
                                        ))}
                                    </Row>
                                ) : (
                                    <Alert color="info">
                                        No products found for "{searchTerm}".
                                    </Alert>
                                )}
                            </CardBody>

                        </div>
                    )}
                </Card>
            </>
        );
    } else if (isError) {
        content =
            error.status === "FETCH_ERROR" ? (
                <p>Server is unreachable. Please try again later.</p>
            ) : (
                <p>{JSON.stringify(error)}</p>
            );
    }

    return content;
};

export default Store;
