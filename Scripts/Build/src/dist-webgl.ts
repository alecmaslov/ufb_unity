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
    rmdirSync
} from "fs";
import path from "path";
import { getBucketManager } from "./AWSBucket";
import { copyDirSync, checkMakeDir } from "./files";

const buildDir = "../../Builds/Build";
const tempDir = "../../Builds/WebGL_Temp";
const outDir = "E:/UFB/ufb-web";

// Before your existing code starts
checkMakeDir(tempDir);
copyDirSync(buildDir, tempDir);

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
        // const oldPath = getFileInBuildDir(file.filename, true, true);
        file.filepath = getFileInBuildDir(file.filename, true, true);
        const stat = statSync(file.filepath!);
        bundleSize += stat.size;

        // const newPath = getFileInBuildDir(file.filename, true, false);
        // file.filepath = newPath;
        // renameSync(oldPath, newPath);
        // console.log(`${oldPath} -> ${file.filepath} | ${stat.size} bytes`);

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

            // const regex = new RegExp(`Build/${prefix}${file.filename}`, "g");
            // const s3Url = data.Location;
            file.filepath = data.Location;
        }

        if (file.includeInPublic) {
            copyFileSync(
                file.filepath!,
                path.join(tempDir, publicFolder, file.filename)
            );
            file.filepath = publicFolder + file.filename;
        }

        // else {
        //     indexHtml = indexHtml.replace(regex, "Build/" + file.filename);
        // }

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
    copyDirSync(tempDir, outDir);
}

processBuild();
