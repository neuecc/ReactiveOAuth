// archive zip for release package
#r "Ionic.Zip.dll"

open System
open System.IO
open System.Text.RegularExpressions
open Ionic.Zip

let rootDir = (new DirectoryInfo(__SOURCE_DIRECTORY__)).Parent
let pass p = Path.Combine(__SOURCE_DIRECTORY__, p)

// traverse
let ignoreDirs = Set([".hg";"Bin";"bin";"obj";"release";"tool";"Binary"])
let rec traverseDir (di:DirectoryInfo) = seq {
    for x in di.EnumerateDirectories() |> Seq.filter (fun x -> not (ignoreDirs.Contains x.Name)) do
        yield x
        for y in (traverseDir x)  do
            yield y}

let ignoreFileExts = Set([".hgignore";".suo";".user"])    
let files = 
    rootDir.EnumerateFiles()
    |> Seq.append (traverseDir rootDir |> Seq.collect (fun x -> x.EnumerateFiles()))
    |> Seq.filter (fun x -> not (ignoreFileExts.Contains x.Extension ))

let binaries =
    Directory.EnumerateFiles(pass "../Binary")
    |> Seq.map (fun x -> new FileInfo(x))
    |> Seq.filter (fun x -> Regex.IsMatch(x.Name, @"^ReactiveOAuth.+"))

// compress
do
    let pathSubtract pathbase target = 
        Regex.Replace(target, "^" + Regex.Escape(pathbase), "")
    use zip = new ZipFile()
    for x in (Seq.append files binaries) do
        zip.AddFile(x.FullName, pathSubtract rootDir.FullName x.DirectoryName) |> ignore
    pass "archive.zip" |> zip.Save