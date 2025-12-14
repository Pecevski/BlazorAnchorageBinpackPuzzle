// Drag and drop interop functions
window.dragDropInterop = {
    setDragData: function (dataTransfer, data) {
        for (let key in data) {
            if (data.hasOwnProperty(key)) {
                dataTransfer.setData(key, data[key]);
            }
        }
    },
    getDragData: function (dataTransfer, key) {
        return dataTransfer.getData(key);
    }
};

window.getElementRect = function (element) {
    const rect = element.getBoundingClientRect();
    return {
        left: rect.left,
        top: rect.top,
        right: rect.right,
        bottom: rect.bottom,
        width: rect.width,
        height: rect.height
    };
};