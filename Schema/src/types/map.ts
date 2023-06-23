import { type } from "os";

export enum TileType {
    Bridge = "Bridge",
    Floor = "Floor",
    Void = "Void",
    Chest = "Chest",
    Enemy = "Enemy",
    Portal = "Portal",
}

export interface TileColor {
    name: string;
    color: string;
}

export interface SpawnEntity {
    name : string;
    // type : TileType;
    properties : any;
}

export interface TileEdge {
    side : "left" | "right" | "top" | "bottom";
    edgeProperty : "wall" | "bridge";
}


export interface Coordinates {
    x: number;
    y: number;
}

export interface GameTile {
    id: string;
    type : TileType;
    spawnItems : SpawnEntity[];
    coordinates : Coordinates;
    color: TileColor | null;
    layerName : string;
    edges : TileEdge[];
    legacyCode: string; // code for the original UFB map parser in Swift
}

export interface UFBMap {
    name : string;
    defaultCards: string[];
    tiles : Partial<GameTile>[];
}
  
export const TileColorCodes = new Map<string, TileColor>([
    ["E", { name: "white", color: "#ffffff" }],
    ["R", { name: "red", color: "#ff493e" }],
    ["G", { name: "green", color: "#00C503" }],
    ["T", { name: "lightBrown", color: "#d09a69" }],
    ["N", { name: "brown", color: "#b26618" }],
    ["S", { name: "gray", color: "#e1e1e1" }],
    ["Y", { name: "yellow", color: "#f9f964" }],
    ["I", { name: "iceBlue", color: "#bdeaee" }],
    ["A", { name: "blue", color: "#0051e0" }],
    ["P", { name: "purple", color: "#c166fe" }],
    ["Q", { name: "lightGreen", color: "#6dff6d" }],
    ["O", { name: "orange", color: "#fba260" }],
    ["L", { name: "teal", color: "#33FFA5" }],
    ["K", { name: "pink", color: "#ffa7ff" }],
]);