import { useParams } from 'react-router-dom';
import { useGetProductByIdQuery } from './catalogApiSlice';
import { ProductDetail } from './components/ProductDetail';

const Product = () => {
    const { productId } = useParams();

    const { data: product, error, isLoading } = useGetProductByIdQuery(productId);

    let content;

    if (isLoading) {
        content = <div>Loading...</div>;
    } else if (error) {
        error.status === 404
            ? content = <div>Product Not found</div>
            : content = <div>Error </div>;
    } else if (product) {
        console.log('Product Data:', product);
        // Display product details here
        content = (
                <ProductDetail key={product.id} product={product} />
        );
    } else {
        content = <div>Product not found</div>;
    }

    return content;
};

export default Product;
