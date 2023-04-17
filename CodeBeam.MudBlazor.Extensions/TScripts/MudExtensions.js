
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
    if (cssprop == "aligncontent") {
        return element.style.alignContent;
    }
    else if (cssprop == "alignitems") {
        return element.style.alignItems;
    }
    else if (cssprop == "alignself") {
        return element.style.alignSelf;
    }
    else if (cssprop == "animation") {
        return element.style.animation;
    }
    else if (cssprop == "animationdelay") {
        return element.style.animationDelay;
    }
    else if (cssprop == "animationdirection") {
        return element.style.animationDirection;
    }
    else if (cssprop == "animationduration") {
        return element.style.animationDuration;
    }
    else if (cssprop == "animationfillmode") {
        return element.style.animationFillMode;
    }
    else if (cssprop == "animationiterationcount") {
        return element.style.animationIterationCount;
    }
    else if (cssprop == "animationname") {
        return element.style.animationName;
    }
    else if (cssprop == "animationplaystate") {
        return element.style.animationPlayState;
    }
    else if (cssprop == "animationtimingfunction") {
        return element.style.animationTimingFunction;
    }
    else if (cssprop == "background") {
        return element.style.background;
    }
    else if (cssprop == "backgroundattachment") {
        return element.style.backgroundAttachment;
    }
    else if (cssprop == "backgroundblendmode") {
        return element.style.backgroundBlendMode;
    }
    else if (cssprop == "backgroundclip") {
        return element.style.backgroundClip;
    }
    else if (cssprop == "backgroundcolor") {
        return element.style.backgroundColor;
    }
    else if (cssprop == "backgroundimage") {
        return element.style.backgroundImage;
    }
    else if (cssprop == "backgroundorigin") {
        return element.style.backgroundOrigin;
    }
    else if (cssprop == "backgroundposition") {
        return element.style.backgroundPosition;
    }
    else if (cssprop == "backgroundrepeat") {
        return element.style.backgroundRepeat;
    }
    else if (cssprop == "backgroundsize") {
        return element.style.backgroundSize;
    }
    else if (cssprop == "border") {
        return element.style.border;
    }
    else if (cssprop == "borderimage") {
        return element.style.borderImage;
    }
    else if (cssprop == "borderbottom") {
        return element.style.borderBottom;
    }
    else if (cssprop == "borderleft") {
        return element.style.borderLeft;
    }
    else if (cssprop == "borderright") {
        return element.style.borderRight;
    }
    else if (cssprop == "bordertop") {
        return element.style.borderTop;
    }
    else if (cssprop == "borderradius") {
        return element.style.borderRadius;
    }
    else if (cssprop == "borderspacing") {
        return element.style.borderSpacing;
    }
    else if (cssprop == "borderstyle") {
        return element.style.borderStyle;
    }
    else if (cssprop == "borderwidth") {
        return element.style.borderWidth;
    }
    else if (cssprop == "bottom") {
        return element.style.bottom;
    }
    else if (cssprop == "boxshadow") {
        return element.style.boxShadow;
    }
    else if (cssprop == "boxsizing") {
        return element.style.boxSizing;
    }
    else if (cssprop == "caretcolor") {
        return element.style.caretColor;
    }
    else if (cssprop == "clip") {
        return element.style.clip;
    }
    else if (cssprop == "color") {
        return element.style.color;
    }
    else if (cssprop == "cursor") {
        return element.style.cursor;
    }
    else if (cssprop == "direction") {
        return element.style.direction;
    }
    else if (cssprop == "display") {
        return element.style.display;
    }
    else if (cssprop == "flex") {
        return element.style.flex;
    }
    else if (cssprop == "flexbasis") {
        return element.style.flexBasis;
    }
    else if (cssprop == "flexdirection") {
        return element.style.flexDirection;
    }
    else if (cssprop == "flexflow") {
        return element.style.flexFlow;
    }
    else if (cssprop == "flexgrow") {
        return element.style.flexGrow;
    }
    else if (cssprop == "flexshrink") {
        return element.style.flexShrink;
    }
    else if (cssprop == "flexwrap") {
        return element.style.flexWrap;
    }
    else if (cssprop == "float") {
        return element.style.float;
    }
    else if (cssprop == "font") {
        return element.style.font;
    }
    else if (cssprop == "fontfamily") {
        return element.style.fontFamily;
    }
    else if (cssprop == "fontkerning") {
        return element.style.fontKerning;
    }
    else if (cssprop == "fontsize") {
        return element.style.fontSize;
    }
    else if (cssprop == "fontsizeadjust") {
        return element.style.fontSizeAdjust;
    }
    else if (cssprop == "fontstretch") {
        return element.style.fontStretch;
    }
    else if (cssprop == "fontstyle") {
        return element.style.fontstyle;
    }
    else if (cssprop == "fontvariant") {
        return element.style.fontVariant;
    }
    else if (cssprop == "fontweight") {
        return element.style.fontWeight;
    }
    else if (cssprop == "grid") {
        return element.style.grid;
    }
    else if (cssprop == "gridarea") {
        return element.style.gridArea;
    }
    else if (cssprop == "gridcolumn") {
        return element.style.gridColumn;
    }
    else if (cssprop == "gridrow") {
        return element.style.gridrow;
    }
    else if (cssprop == "gridtemplate") {
        return element.style.gridTemplate;
    }
    else if (cssprop == "gridtemplateareas") {
        return element.style.gridTemplateAreas;
    }
    else if (cssprop == "gridtemplatecolumns") {
        return element.style.gridTemplateColumns;
    }
    else if (cssprop == "gridtemplaterows") {
        return element.style.gridTemplateRows;
    }
    else if (cssprop == "height") {
        return element.style.height;
    }
    else if (cssprop == "hyphens") {
        return element.style.hyphens;
    }
    else if (cssprop == "justifycontent") {
        return element.style.justifyContent;
    }
    else if (cssprop == "left") {
        return element.style.left;
    }
    else if (cssprop == "letterspacing") {
        return element.style.letterSpacing;
    }
    else if (cssprop == "lineheight") {
        return element.style.lineHeight;
    }
    else if (cssprop == "liststyle") {
        return element.style.listStyle;
    }
    else if (cssprop == "margin") {
        return element.style.margin;
    }
    else if (cssprop == "marginbottom") {
        return element.style.marginBottom;
    }
    else if (cssprop == "marginleft") {
        return element.style.marginLeft;
    }
    else if (cssprop == "marginright") {
        return element.style.marginRight;
    }
    else if (cssprop == "margintop") {
        return element.style.marginTop;
    }
    else if (cssprop == "maxheight") {
        return element.style.maxHeight;
    }
    else if (cssprop == "maxwidth") {
        return element.style.maxWidth;
    }
    else if (cssprop == "minheight") {
        return element.style.minHeight;
    }
    else if (cssprop == "minwidth") {
        return element.style.minWidth;
    }
    else if (cssprop == "objectfit") {
        return element.style.objectFit;
    }
    else if (cssprop == "objectposition") {
        return element.style.objectPosition;
    }
    else if (cssprop == "opacity") {
        return element.style.opacity;
    }
    else if (cssprop == "order") {
        return element.style.order;
    }
    else if (cssprop == "outline") {
        return element.style.outline;
    }
    else if (cssprop == "overflow") {
        return element.style.overflow;
    }
    else if (cssprop == "overflowx") {
        return element.style.overflowX;
    }
    else if (cssprop == "overflowy") {
        return element.style.overflowY;
    }
    else if (cssprop == "padding") {
        return element.style.padding;
    }
    else if (cssprop == "paddingbottom") {
        return element.style.paddingBottom;
    }
    else if (cssprop == "paddingleft") {
        return element.style.paddingLeft;
    }
    else if (cssprop == "paddingright") {
        return element.style.paddingRight;
    }
    else if (cssprop == "paddingtop") {
        return element.style.paddingTop;
    }
    else if (cssprop == "pointerevents") {
        return element.style.pointerEvents;
    }
    else if (cssprop == "position") {
        return element.style.position;
    }
    else if (cssprop == "right") {
        return element.style.right;
    }
    else if (cssprop == "top") {
        return element.style.top;
    }
    else if (cssprop == "transform") {
        return element.style.transform;
    }
    else if (cssprop == "transition") {
        return element.style.transition;
    }
    else if (cssprop == "userselect") {
        return element.style.userSelect;
    }
    else if (cssprop == "verticalalign") {
        return element.style.verticalAlign;
    }
    else if (cssprop == "visibility") {
        return element.style.visibility;
    }
    else if (cssprop == "whitespace") {
        return element.style.whiteSpace;
    }
    else if (cssprop == "wordbreak") {
        return element.style.wordBreak;
    }
    else if (cssprop == "wordspacing") {
        return element.style.wordSpacing;
    }
    else if (cssprop == "wordwrap") {
        return element.style.wordWrap;
    }
    else if (cssprop == "writingmode") {
        return element.style.writingMode;
    }
    else if (cssprop == "width") {
        return element.style.width;
    }
    else if (cssprop == "zindex") {
        return element.style.zIndex;
    }
}

function setcss(classe, cssprop, value) {
    const elements = document.querySelectorAll(classe);
    if (cssprop == "aligncontent") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.alignContent = value;
        }
    }
    else if (cssprop == "alignitems") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.alignItems = value;
        }
    }
    else if (cssprop == "alignself") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.alignSelf = value;
        }
    }
    else if (cssprop == "animation") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.animation = value;
        }
    }
    else if (cssprop == "animationdelay") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.animationDelay = value;
        }
    }
    else if (cssprop == "animationdirection") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.animationDirection = value;
        }
    }
    else if (cssprop == "animationduration") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.animationDuration = value;
        }
    }
    else if (cssprop == "animationfillmode") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.animationFillMode = value;
        }
    }
    else if (cssprop == "animationiterationcount") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.animationIterationCount = value;
        }
    }
    else if (cssprop == "animationname") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.animationName = value;
        }
    }
    else if (cssprop == "animationplaystate") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.animationPlayState = value;
        }
    }
    else if (cssprop == "animationtimingfunction") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.animationTimingFunction = value;
        }
    }
    else if (cssprop == "background") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.background = value;
        }
    }
    else if (cssprop == "backgroundattachment") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.backgroundAttachment = value;
        }
    }
    else if (cssprop == "backgroundblendmode") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.backgroundBlendMode = value;
        }
    }
    else if (cssprop == "backgroundclip") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.backgroundClip = value;
        }
    }
    else if (cssprop == "backgroundcolor") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.backgroundColor = value;
        }
    }
    else if (cssprop == "backgroundimage") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.backgroundImage = value;
        }
    }
    else if (cssprop == "backgroundorigin") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.backgroundOrigin = value;
        }
    }
    else if (cssprop == "backgroundposition") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.backgroundPosition = value;
        }
    }
    else if (cssprop == "backgroundrepeat") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.backgroundRepeat = value;
        }
    }
    else if (cssprop == "backgroundsize") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.backgroundSize = value;
        }
    }
    else if (cssprop == "border") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.border = value;
        }
    }
    else if (cssprop == "borderimage") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.borderImage = value;
        }
    }
    else if (cssprop == "borderbottom") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.borderBottom = value;
        }
    }
    else if (cssprop == "borderleft") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.borderLeft = value;
        }
    }
    else if (cssprop == "borderright") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.borderRight = value;
        }
    }
    else if (cssprop == "bordertop") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.borderTop = value;
        }
    }
    else if (cssprop == "borderradius") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.borderRadius = value;
        }
    }
    else if (cssprop == "borderspacing") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.borderSpacing = value;
        }
    }
    else if (cssprop == "borderstyle") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.borderStyle = value;
        }
    }
    else if (cssprop == "borderwidth") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.borderWidth = value;
        }
    }
    else if (cssprop == "bottom") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.bottom = value;
        }
    }
    else if (cssprop == "boxshadow") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.boxShadow = value;
        }
    }
    else if (cssprop == "boxSizing") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.boxSizing = value;
        }
    }
    else if (cssprop == "caretcolor") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.caretColor = value;
        }
    }
    else if (cssprop == "clip") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.clip = value;
        }
    }
    else if (cssprop == "color") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.color = value;
        }
    }
    else if (cssprop == "cursor") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.cursor = value;
        }
    }
    else if (cssprop == "height") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.height = value;
        }
    }
    else if (cssprop == "direction") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.direction = value;
        }
    }
    else if (cssprop == "display") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.display = value;
        }
    }
    else if (cssprop == "filter") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.filter = value;
        }
    }
    else if (cssprop == "flex") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.flex = value;
        }
    }
    else if (cssprop == "flexbasis") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.flexBasis = value;
        }
    }
    else if (cssprop == "flexdirection") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.flexDirection = value;
        }
    }
    else if (cssprop == "flexflow") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.flexFlow = value;
        }
    }
    else if (cssprop == "flexgrow") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.flexGrow = value;
        }
    }
    else if (cssprop == "flexshrink") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.flexShrink = value;
        }
    }
    else if (cssprop == "flexwrap") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.flexWrap = value;
        }
    }
    else if (cssprop == "float") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.float = value;
        }
    }
    else if (cssprop == "font") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.font = value;
        }
    }
    else if (cssprop == "fontfamily") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.fontFamily = value;
        }
    }
    else if (cssprop == "fontkerning") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.fontKerning = value;
        }
    }
    else if (cssprop == "fontsize") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.fontSize = value;
        }
    }
    else if (cssprop == "fontsizeadjust") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.fontSizeAdjust = value;
        }
    }
    else if (cssprop == "fontstretch") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.fontStretch = value;
        }
    }
    else if (cssprop == "fontstyle") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.fontStyle = value;
        }
    }
    else if (cssprop == "fontvariant") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.fontVariant = value;
        }
    }
    else if (cssprop == "fontweight") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.fontWeight = value;
        }
    }
    else if (cssprop == "grid") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.grid = value;
        }
    }
    else if (cssprop == "gridarea") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.gridArea = value;
        }
    }
    else if (cssprop == "gridcolumn") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.gridColumn = value;
        }
    }
    else if (cssprop == "gridrow") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.gridRow = value;
        }
    }
    else if (cssprop == "gridtemplate") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.gridTemplate = value;
        }
    }
    else if (cssprop == "gridtemplateareas") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.gridTemplateAreas = value;
        }
    }
    else if (cssprop == "gridtemplatecolumns") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.gridTemplateColumns = value;
        }
    }
    else if (cssprop == "gridtemplaterows") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.gridTemplateRows = value;
        }
    }
    else if (cssprop == "height") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.height = value;
        }
    }
    else if (cssprop == "hyphens") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.hyphens = value;
        }
    }
    else if (cssprop == "justifycontent") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.justifyContent = value;
        }
    }
    else if (cssprop == "left") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.left = value;
        }
    }
    else if (cssprop == "letterspacing") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.letterSpacing = value;
        }
    }
    else if (cssprop == "lineheight") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.lineHeight = value;
        }
    }
    else if (cssprop == "liststyle") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.listStyle = value;
        }
    }
    else if (cssprop == "margin") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.margin = value;
        }
    }
    else if (cssprop == "marginbottom") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.marginBottom = value;
        }
    }
    else if (cssprop == "marginleft") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.marginLeft = value;
        }
    }
    else if (cssprop == "marginright") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.marginRight = value;
        }
    }
    else if (cssprop == "margintop") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.marginTop = value;
        }
    }
    else if (cssprop == "maxheight") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.maxHeight = value;
        }
    }
    else if (cssprop == "maxwidth") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.maxWidth = value;
        }
    }
    else if (cssprop == "minheight") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.minHeight = value;
        }
    }
    else if (cssprop == "minwidth") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.minWidth = value;
        }
    }
    else if (cssprop == "objectfit") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.objectFit = value;
        }
    }
    else if (cssprop == "objectposition") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.objectPosition = value;
        }
    }
    else if (cssprop == "opacity") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.opacity = value;
        }
    }
    else if (cssprop == "order") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.order = value;
        }
    }
    else if (cssprop == "outline") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.outline = value;
        }
    }
    else if (cssprop == "overflow") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.overflow = value;
        }
    }
    else if (cssprop == "overflowx") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.overflowX = value;
        }
    }
    else if (cssprop == "overflowy") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.overflowY = value;
        }
    }
    else if (cssprop == "padding") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.padding = value;
        }
    }
    else if (cssprop == "paddingbottom") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.paddingBottom = value;
        }
    }
    else if (cssprop == "paddingleft") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.paddingLeft = value;
        }
    }
    else if (cssprop == "paddingright") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.paddingRight = value;
        }
    }
    else if (cssprop == "paddingtop") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.paddingTop = value;
        }
    }
    else if (cssprop == "pointerevents") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.pointerEvents = value;
        }
    }
    else if (cssprop == "position") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.position = value;
        }
    }
    else if (cssprop == "right") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.right = value;
        }
    }
    else if (cssprop == "top") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.top = value;
        }
    }
    else if (cssprop == "tranform") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.transform = value;
        }
    }
    else if (cssprop == "transition") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.transition = value;
        }
    }
    else if (cssprop == "userselect") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.userSelect = value;
        }
    }
    else if (cssprop == "verticalalign") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.verticalAlign = value;
        }
    }
    else if (cssprop == "visibility") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.visibility = value;
        }
    }
    else if (cssprop == "whitespace") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.whiteSpace = value;
        }
    }
    else if (cssprop == "wordbreak") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.wordBreak = value;
        }
    }
    else if (cssprop == "wordspacing") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.wordSpacing = value;
        }
    }
    else if (cssprop == "wordwrap") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.wordWrap = value;
        }
    }
    else if (cssprop == "writingmode") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.writingMode = value;
        }
    }
    else if (cssprop == "width") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.width = value;
        }
    }
    else if (cssprop == "zindex") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.zIndex = value;
        }
    }
}