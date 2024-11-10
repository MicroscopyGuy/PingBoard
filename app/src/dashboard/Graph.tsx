/**
import { useRef } from 'react';
import ReactDOM from 'react-dom/client'

function ShadowContainer() {
    const divRef = useRef();
    return (
        <div ref={(div) => {
            if (!div) return;
            const shadowRoot = div.attachShadow();
            
        }}></div>
    )
}

interface GraphProps { }
function Graph(p: GraphProps) {

}
*/