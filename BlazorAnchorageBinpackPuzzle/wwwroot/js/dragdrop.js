// All drag-and-drop is handled in JS to avoid Blazor event system interference.
// Blazor is only called back via DotNetObjectReference on a successful drop.

window.getElementRect = function (element) {
    var rect = element.getBoundingClientRect();
    return { left: rect.left, top: rect.top };
};

window.initGridDropZone = function (gridElement, dotNetRef) {
    gridElement.addEventListener('dragover', function (e) {
        e.preventDefault();
        e.dataTransfer.dropEffect = 'move';
    });

    gridElement.addEventListener('drop', function (e) {
        e.preventDefault();
        var shipDesignation = e.dataTransfer.getData('ship-designation');
        var isRotated = e.dataTransfer.getData('is-rotated') === 'True';

        if (!shipDesignation) return;

        var rect = gridElement.getBoundingClientRect();
        var gridX = Math.floor((e.clientX - rect.left) / 40);
        var gridY = Math.floor((e.clientY - rect.top) / 40);

        dotNetRef.invokeMethodAsync('OnDropFromJs', gridX, gridY, shipDesignation, isRotated)
            .catch(function (err) { console.error('Drop error:', err); });
    });
};

// Handle dragstart for all vessel items via data attributes.
document.addEventListener('dragstart', function (e) {
    var vesselItem = e.target.closest('.vessel-item');
    if (vesselItem) {
        e.dataTransfer.setData('ship-designation', vesselItem.getAttribute('data-ship-designation') || '');
        e.dataTransfer.setData('vessel-index', vesselItem.getAttribute('data-vessel-index') || '0');
        e.dataTransfer.setData('is-rotated', vesselItem.getAttribute('data-is-rotated') || 'False');
        e.dataTransfer.effectAllowed = 'move';
    }
});
