class MudScrollManagerExtended {

    scrollToMiddle(parentId, childId) {

        let parent = document.getElementById(parentId);
        let child = document.getElementById(childId);

        parent.scrollTop = (child.offsetTop - parent.offsetHeight) + (parent.offsetHeight / 2) + (child.offsetHeight / 2);
    }
};
window.mudScrollManagerExtended = new MudScrollManagerExtended();

function auto_size(element) {
    element.style.height = "5px";
    element.style.height = (element.scrollHeight + 4) + "px";
}

function getcss(classe, cssprop) {
    const element = document.querySelector(classe);
    return element.style.getPropertyValue(cssprop);
}

function setcss(classe, cssprop, value) {
    const elements = document.querySelectorAll(classe);

    for (let i = 0; i < elements.length; i++) {
        elements[i].style.setProperty(cssprop, value);
    }
}


window.mudTeleport = {
    teleport: (source, to) => {
        const target = document.querySelector(to);
        if (!target) {
            //throw new Error(`teleport: ${to} is not found on the DOM`);
            return "not found";
        }
        target.appendChild(source);
        return "ok";
    },

    removeFromDOM: (el) => {
        if (el && el.__internalId !== null) el.remove();
    },
};

class MudSignaturePadManager {
    constructor() {
        this.pads = [];
    }

    addPad(dotnetRef, canvasRef, canvasOption) {
        const signaturePad = new MudSignaturePad(dotnetRef, canvasRef, canvasOption);
        signaturePad.init();
        this.pads.push(signaturePad);
    }

    togglePadEraser(canvasRef) {
        const pad = this.getPad(canvasRef);
        if (pad) pad.toggleEraser();
    }

    disposePad(canvasRef) {
        const pad = this.getPad(canvasRef);
        if (pad) pad.dispose();
    }

    clearPad(canvasRef) {
        const pad = this.getPad(canvasRef);
        if (pad) pad.clear(true);
    }

    downloadPadImage(canvasRef) {
        const pad = this.getPad(canvasRef);
        if (pad) pad.download();
    }

    getBase64(canvasRef) {
        const pad = this.getPad(canvasRef);
        if (pad) return pad.getBase64();
    }

    updatePadOptions(canvasRef, options) {
        const pad = this.getPad(canvasRef);
        if (pad) pad.setOptions(options);
    }

    updatePadImage(canvasRef, base64Src) {
        const pad = this.getPad(canvasRef);
        if (pad) {
            if (!base64Src.startsWith("data:image/png;base64,")) {
                base64Src = `data:image/png;base64,${base64Src}`;
            }
            pad.updateImage(base64Src);
        }
    }

    setCanvasSize(canvasRef) {
        const pad = this.getPad(canvasRef);
        if (pad) {
            pad.updateCanvasSize();
        }
    }

    getPad(canvasRef) {
        return this.pads.find(x => x.canvas.id === canvasRef.id) || null;
    }
}

class MudSignaturePad {
    constructor(dotnetRef, canvasRef, canvasOption) {
        this.canvas = canvasRef;
        this.options = canvasOption;
        this.isMouseDown = false;
        this.isErasing = false;
        this.memCanvas = document.createElement('canvas');
        this.points = [];
        this.dotnetRef = dotnetRef;
        this.onPointerDown = this.handlePointerDown.bind(this);
        this.onPointerMove = this.handlePointerMove.bind(this);
        this.onPointerUp = this.handlePointerUp.bind(this);
        this.onPointerLeave = this.stopDrawing.bind(this);
    }

    get ctx() {
        return this.canvas.getContext('2d');
    }

    get memCtx() {
        return this.memCanvas.getContext('2d');
    }

    init() {
        this.setCanvasSize();
        this.setOptions(this.options);

        this.canvas.addEventListener("pointerdown", this.onPointerDown, { passive: false });
        this.canvas.addEventListener("pointermove", this.onPointerMove, { passive: false });
        this.canvas.addEventListener("pointerup", this.onPointerUp, { passive: false });
        this.canvas.addEventListener("pointerleave", this.onPointerLeave);

        this.canvas.style.touchAction = 'none';
        this.setPencilCursor();
    }

    dispose() {
        this.canvas.removeEventListener("pointerdown", this.onPointerDown);
        this.canvas.removeEventListener("pointermove", this.onPointerMove);
        this.canvas.removeEventListener("pointerup", this.onPointerUp);
        this.canvas.removeEventListener("pointerleave", this.onPointerLeave);
    }

    setCanvasSize() {
        const parent = this.canvas.parentElement;
        if (!parent) return;
        const parentRect = parent.getBoundingClientRect();
        this.canvas.width = parentRect.width;
        this.canvas.height = parentRect.height;
        this.memCanvas.width = parentRect.width;
        this.memCanvas.height = parentRect.height;
    }

    updateCanvasSize() {
        const parent = this.canvas.parentElement;
        if (!parent) return;

        const parentRect = parent.getBoundingClientRect();
        const newWidth = parentRect.width;
        const newHeight = parentRect.height;

        // 1. Mevcut çizimi geçici bir canvas'a taşı
        const oldCanvas = document.createElement('canvas');
        oldCanvas.width = this.canvas.width;
        oldCanvas.height = this.canvas.height;
        const oldCtx = oldCanvas.getContext('2d');
        oldCtx.drawImage(this.canvas, 0, 0);

        // 2. Canvas boyutunu güncelle
        this.canvas.width = newWidth;
        this.canvas.height = newHeight;
        this.memCanvas.width = newWidth;
        this.memCanvas.height = newHeight;

        // 3. Eski çizimi geri aktar
        this.ctx.drawImage(oldCanvas, 0, 0);
        this.memCtx.drawImage(oldCanvas, 0, 0);
    }


    getBase64() {
        return this.canvas.toDataURL();
    }

    updateImage(base64) {
        this.clear(true);
        const image = new Image();
        image.onload = () => {
            this.ctx.drawImage(image, 0, 0);
            this.memCtx.drawImage(image, 0, 0);
        };
        image.src = base64;
    }

    download() {
        const link = document.createElement('a');
        link.download = 'signature.png';
        link.href = this.getBase64();
        link.click();
        link.remove();
    }

    clear(both) {
        this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
        if (both) {
            this.memCtx.clearRect(0, 0, this.canvas.width, this.canvas.height);
        }
    }

    setOptions(options) {
        this.options = options;

        const applyOptions = (ctx) => {
            ctx.lineWidth = options.lineWidth;
            ctx.lineJoin = options.lineJoin;
            ctx.lineCap = options.lineCap;
            ctx.strokeStyle = options.strokeStyle;
        };

        applyOptions(this.ctx);
        applyOptions(this.memCtx);
    }


    toggleEraser() {
        this.isErasing = !this.isErasing;
        this.isErasing ? this.setEraserCursor() : this.setPencilCursor();
    }

    setPencilCursor() {
        this.canvas.style.cursor = "url('_content/CodeBeam.MudBlazor.Extensions/pencil.cur'), auto";
    }

    setEraserCursor() {
        this.canvas.style.cursor = "url('_content/CodeBeam.MudBlazor.Extensions/eraser.cur'), auto";
    }

    handlePointerDown(e) {
        e.preventDefault();
        this.isMouseDown = true;
        const { offsetX, offsetY } = e;
        this.points = [{ x: offsetX, y: offsetY }];
    }

    handlePointerMove(e) {
        if (!this.isMouseDown) return;
        e.preventDefault();

        const { offsetX, offsetY } = e;
        if (!this.isErasing) {
            this.clear();
            this.ctx.drawImage(this.memCanvas, 0, 0);
            this.points.push({ x: offsetX, y: offsetY });
            this.drawPoints(this.ctx, this.points);
        } else {
            this.ctx.clearRect(offsetX - 10, offsetY - 10, 23, 23);
        }
    }

    handlePointerUp(e) {
        e.preventDefault();
        this.stopDrawing();
    }

    stopDrawing() {
        if (!this.isMouseDown) return;
        this.isMouseDown = false;
        this.memCtx.clearRect(0, 0, this.memCanvas.width, this.memCanvas.height);
        this.memCtx.drawImage(this.canvas, 0, 0);
        this.points = [];
    }

    drawPoints(ctx, points) {
        if (points.length < 2) return;

        if (points.length < 6) {
            const p = points[0];
            ctx.beginPath();
            ctx.lineWidth = this.options.lineWidth;
            ctx.strokeStyle = this.options.strokeStyle;
            ctx.arc(p.x, p.y, ctx.lineWidth / 2, 0, Math.PI * 2, true);
            ctx.fill();
            ctx.closePath();
            this.pushUpdateToBlazorComponent();
            return;
        }

        ctx.beginPath();
        ctx.moveTo(points[0].x, points[0].y);

        for (let i = 1; i < points.length - 2; i++) {
            const c = (points[i].x + points[i + 1].x) / 2;
            const d = (points[i].y + points[i + 1].y) / 2;
            ctx.quadraticCurveTo(points[i].x, points[i].y, c, d);
        }

        ctx.quadraticCurveTo(
            points[points.length - 2].x,
            points[points.length - 2].y,
            points[points.length - 1].x,
            points[points.length - 1].y
        );

        ctx.stroke();
        this.pushUpdateToBlazorComponent();
    }

    pushUpdateToBlazorComponent() {
        this.dotnetRef.invokeMethodAsync("SignatureDataChangedAsync");
    }
}

window.mudSignaturePad = new MudSignaturePadManager();
