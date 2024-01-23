import { useGetCatalogQuery } from "./catalogApiSlice";
import { CatalogProduct } from "./components/CatalogProduct";
import { Row } from "reactstrap";

const Catalog = () => {
    const {
        data: catalog,
        isLoading,
        isSuccess,
        isError,
        error
    } = useGetCatalogQuery()

    let content;

    if (isLoading) {
        content = <p>Loading...</p>
    } else if (isSuccess) {
            const products = catalog.filter(product => !product.inactive);
            const rows = [...Array(Math.ceil(products.length / 4))];
            const productRows = rows.map((row, idx) => products.slice(idx * 4, idx * 4 + 4));
            
            content = productRows.map((row, idx) => (
                <Row key={idx} lg="4" sm="2" xs="1">
                    {row
                        .map(product => <CatalogProduct key={product.id} product={product} />)
                    }
                </Row>
            

        ));

    } else if (isError) {
        error.status === "FETCH_ERROR" 
            ? content = <p>Server is unreachable, Please try again later.</p>
            : content = <p>{JSON.stringify(error)}</p>

    }

    // return content
    return content;

}

export default Catalog;