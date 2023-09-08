import { mkdirSync, copyFileSync, existsSync, readdirSync, statSync } from "fs";
import path from "path";

export function copyDirSync(source: string, destination: string) {
    if (!existsSync(destination)) {
        mkdirSync(destination);
    }

    const files = readdirSync(source);

    for (const file of files) {
        const srcFile = path.join(source, file);
        const destFile = path.join(destination, file);

        const stat = statSync(srcFile);

        if (stat.isDirectory()) {
            copyDirSync(srcFile, destFile);
        } else {
            copyFileSync(srcFile, destFile);
        }
    }
}

export function checkMakeDir(dir: string) {
    if (!existsSync(dir)) {
        mkdirSync(dir);
    }
}
