  export const formatDate = (timestamp) => {
    const orderDate = new Date(timestamp);
  
    const formattedDate = orderDate.toLocaleString('tr-TR', {
      day: 'numeric',
      month: 'long',
    // Include the year if it's different from the current year
    year: orderDate.getFullYear() !== new Date().getFullYear() ? 'numeric' : undefined
    });
  
    return formattedDate;
  };

  export const formatTime = (timestamp) => {
    const orderDate = new Date(timestamp);
  
    const formattedDate = orderDate.toLocaleString('tr-TR', {
      hour: '2-digit',
      minute: '2-digit',
    });
  
    return formattedDate;
  };

  