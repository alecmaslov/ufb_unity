import dotenv from "dotenv";
dotenv.config();

import { S3 } from "aws-sdk";
import {
    mkdirSync,
    copyFileSync,
    renameSync,
    statSync,
    readFileSync,
    writeFileSync,
    rmdirSync,
    rmSync,
    existsSync,
} from "fs";
import path from "path";
import { getBucketManager } from "./AWSBucket";
import { copyDirSync, checkMakeDir } from "./files";

const buildDir = "../../Builds/WebGL/WebGL-Build";
const tempDir = "../../Builds/temp";
const outDir = process.env.BUILD_OUT_DIR as string;

// Before your existing code starts
checkMakeDir(tempDir);

// clear temp dir
// rmdirSync(path.join(tempDir, "public"));

if (!existsSync(tempDir)) {
    mkdirSync(tempDir);
} else {
    console.log("Clearing temp dir...");
    rmdirSync(tempDir, { recursive: true });
    mkdirSync(tempDir);
}

copyDirSync(buildDir, tempDir);
// copyDirSync(path.join(buildDir, "StreamingAssets"), tempDir);

interface BuildFile {
    filename: string;
    filepath?: string;
    uploadParams?: Omit<S3.Types.PutObjectRequest, "Bucket" | "Body">;
    includeInPublic?: boolean;
}

const s3Folder = "Build/";
const publicFolder = "public/";

async function processBuild() {
    function getFileInBuildDir(
        filename: string,
        inBuild = true,
        usesPrefix = true
    ) {
        if (usesPrefix) {
            filename = (buildDir.split("/").pop() as string) + "." + filename;
        }
        if (inBuild) {
            return path.join(tempDir, "Build", filename);
        }
        return path.join(tempDir, filename);
    }

    const targetBuildFiles: BuildFile[] = [
        {
            filename: "data.gz",
            uploadParams: {
                Key: s3Folder + "data.gz",
                ContentType: "application/x-gzip",
                ContentEncoding: "gzip",
            },
            includeInPublic: false,
        },

        {
            filename: "framework.js.gz",
            uploadParams: {
                Key: s3Folder + "framework.js.gz",
                ContentType: "text/javascript",
                ContentEncoding: "gzip",
            },
            includeInPublic: false,
        },
        {
            filename: "wasm.gz",
            uploadParams: {
                Key: s3Folder + "wasm.gz",
                ContentType: "application/x-gzip",
                ContentEncoding: "gzip",
            },
            includeInPublic: false,
        },
        {
            filename: "loader.js",
            includeInPublic: true,
        },
    ];

    const prefix = (buildDir.split("/").pop() as string) + ".";

    const bucketManager = getBucketManager(
        process.env.AWS_BUCKET_NAME as string
    );

    let indexHtml = readFileSync(
        getFileInBuildDir("index.html", false, false),
        "utf8"
    );

    checkMakeDir(path.join(tempDir, publicFolder));

    let bundleSize = 0;

    for (const file of targetBuildFiles) {
        file.filepath = getFileInBuildDir(file.filename, true, true);
        const stat = statSync(file.filepath!);
        bundleSize += stat.size;

        if (file.uploadParams) {
            const fileContent = readFileSync(file.filepath!);
            const params: any = {
                ...file.uploadParams,
                Body: fileContent,
            };
            const data = await bucketManager.uploadPromise(
                params,
                (progress) => {
                    console.log(
                        `Uploading ${file.filename}... ${progress.loaded}/${progress.total}`
                    );
                }
            );

            file.filepath = data.Location;
        }

        if (file.includeInPublic) {
            copyFileSync(
                file.filepath!,
                path.join(tempDir, publicFolder, file.filename)
            );
            file.filepath = publicFolder + file.filename;
        }

        const regex = new RegExp(`Build/${prefix}${file.filename}`, "g");
        indexHtml = indexHtml.replace(regex, file.filepath);
    }

    writeFileSync(
        getFileInBuildDir("index.html", false, false),
        indexHtml,
        "utf8"
    );

    rmdirSync(path.join(tempDir, "Build"), { recursive: true });

    checkMakeDir(outDir);
    console.log(`Saving to ${outDir} | Bundle size: ${bundleSize / 1024} KB`);

    rmdirSync(path.join(outDir, "StreamingAssets"), { recursive: true });
    copyDirSync(tempDir, outDir);
}

processBuild();
