export function getProductDetailImageUrl(productid) {
    return (
        'https://picsum.photos/id/' +
        productid +
        '/600/600'
    );
}

export function getProductImageUrl(productid) {
    return (
        'https://picsum.photos/id/' +
        productid +
        '/300/200'
    );
}

export function getProductThumbnail(productid){
    return (
        'https://picsum.photos/id/' +
        productid +
        '/100/75'
    );
}

export function getOrderProductThumbnail(productid){
    return (
        'https://picsum.photos/id/' +
        productid +
        '/50/50'
    );
}