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
        if (pad) {
            pad.toggleEraser();
        }
    }

    disposePad(canvasRef) {
        const pad = this.getPad(canvasRef);
        if (pad) {
            pad.dispose();
        }
    }

    clearPad(canvasRef) {
        const pad = this.getPad(canvasRef);
        if (pad) {
            pad.clear(true);
        }
    }

    downloadPadImage(canvasRef) {
        const pad = this.getPad(canvasRef);
        if (pad) {
            pad.download();
        }
    }

    getBase64(canvasRef) {
        const pad = this.getPad(canvasRef);
        if (pad) {
            return pad.getBase64();
        }
    }

    updatePadOptions(canvasRef, options) {
        const pad = this.getPad(canvasRef);
        if (pad) {
            pad.setOptions(options);
        }
    }

    updatePadImage(canvasRef, base64Src) {
        const pad = this.getPad(canvasRef);
        if (pad) {
            if (base64Src.startsWith("data:image/png;base64,")) {
                pad.updateImage(base64Src);
                return;
            }
            pad.updateImage(`data:image/png;base64,${base64Src}`);
        }
    }

    getPad(canvasRef) {
        const padIndex = this.pads.findIndex(x => x.canvas.id === canvasRef.id);
        if (padIndex >= 0) {
            return this.pads[padIndex];
        }
        return null;
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
    }

    get ctx() {
        return this.canvas.getContext('2d');
    }

    get memCtx() {
        return this.memCanvas.getContext('2d');
    }

    getBase64() {
        return this.canvas.toDataURL();
    }

    init() {
        this.setCanvasSize();
        this.setOptions(this.options);
        this.canvas.addEventListener('mousedown', (e) => this.startDrawing(e));
        this.canvas.addEventListener('mousemove', (e) => this.drawLine(e));
        this.canvas.addEventListener('mouseup', () => this.stopDrawing());
        this.canvas.addEventListener('mouseout', () => this.stopDrawing());
        this.canvas.addEventListener("touchstart", (e) => this.startDrawing(e));
        this.canvas.addEventListener("touchend", () => this.stopDrawing());
        this.canvas.addEventListener("touchmove", (e) => this.drawLine(e));
        this.setPencilCursor();
    };

    download() {
        const link = document.createElement('a');
        link.download = 'signature.png';
        link.href = this.canvas.toDataURL();
        link.click();
        link.remove();
    };

    updateImage(base64) {
        this.clear(true);
        const image = new Image();
        const ctx = this.ctx;
        const memCtx = this.memCtx;
        image.onload = function () {
            ctx.drawImage(image, 0, 0);
            memCtx.drawImage(image, 0, 0);
            image.remove();
        };
        image.src = base64;
    }

    setCanvasSize() {
        const parent = this.canvas.parentElement;
        const parentRect = parent.getBoundingClientRect();
        this.canvas.width = parentRect.width;
        this.canvas.height = parentRect.height;
        this.memCanvas.height = parentRect.height;
        this.memCanvas.width = parentRect.width;
    }

    dispose() {
        this.canvas.removeEventListener('mousedown');
        this.canvas.removeEventListener('mousemove');
        this.canvas.removeEventListener('mouseup');
        this.canvas.removeEventListener('mouseout');
        this.canvas.removeEventListener("touchstart");
        this.canvas.removeEventListener("touchend");
        this.canvas.removeEventListener("touchmove");
    }

    clear(both) {
        if (both === true) {
            this.memCtx.clearRect(0, 0, this.canvas.width, this.canvas.height);
        }
        this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
    }

    stopDrawing() {
        this.isMouseDown = false;
        this.memCtx.clearRect(0, 0, this.memCanvas.width, this.memCanvas.height);
        this.memCtx.drawImage(this.canvas, 0, 0);
        this.points = [];
    }

    startDrawing(event) {
        this.isMouseDown = true;
        this.points.push({
            x: event.offsetX,
            y: event.offsetY
        });
    }

    setOptions(options) {
        this.ctx.lineWidth = options.lineWidth;
        this.ctx.lineJoin = options.lineJoin;
        this.ctx.lineCap = options.lineCap;
        this.ctx.strokeStyle = options.strokeStyle;
        this.options = options;
    }

    toggleEraser() {
        this.isErasing = !this.isErasing;
        if (this.isErasing) {
            this.setEraserCursor();
            return;
        }
        this.setPencilCursor();
    }

    setPencilCursor() {
        this.canvas.setAttribute('style', 'cursor:url(\'_content/CodeBeam.MudBlazor.Extensions/pencil.cur\'), auto;');
    }

    setEraserCursor() {
        this.canvas.setAttribute('style', 'cursor:url(\'_content/CodeBeam.MudBlazor.Extensions/eraser.cur\'), auto;');
    }

    drawLine(event) {
        if (this.isMouseDown) {
            if (this.isErasing === false) {
                this.clear();
                this.ctx.drawImage(this.memCanvas, 0, 0);
                this.points.push({
                    x: event.offsetX,
                    y: event.offsetY
                });
                this.drawPoints(this.ctx, this.points);
            } else {
                this.ctx.clearRect(event.offsetX, event.offsetY, 23, 23);
            }
        }
    }

    drawPoints(ctx, points) {
        if (points.length < 6) return;
        if (points.length < 6) {
            const b = points[0];
            ctx.beginPath();
            ctx.arc(b.x, b.y, ctx.lineWidth / 2, 0, Math.PI * 2, !0);
            ctx.closePath();
            ctx.fill();
            this.pushUpdateToBlazorComponent();
            return;
        }
        ctx.beginPath();
        ctx.moveTo(points[0].x, points[0].y);
        let lastPoint;
        for (let i = 1; i < points.length - 2; i++) {
            const c = (points[i].x + points[i + 1].x) / 2,
                d = (points[i].y + points[i + 1].y) / 2;
            ctx.quadraticCurveTo(points[i].x, points[i].y, c, d);
            lastPoint = i;
        }
        ctx.quadraticCurveTo(points[lastPoint].x, points[lastPoint].y, points[lastPoint + 1].x, points[lastPoint + 1].y);
        ctx.stroke()
        this.pushUpdateToBlazorComponent();
    }

    pushUpdateToBlazorComponent() {
        this.dotnetRef.invokeMethodAsync('SignatureDataChangedAsync');
    }
}

window.mudSignaturePad = new MudSignaturePadManager();