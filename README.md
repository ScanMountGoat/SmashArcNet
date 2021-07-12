# SmashArcNet
A safe C# wrapper for the [smash-arc](https://github.com/jam1garner/smash-arc) Rust library. The project targets .NET Core 3.1 and includes binaries for the Rust library for Windows and Linux. 

## Example Usage
Updated hashes can be found in the [archive-hashes](https://github.com/ultimate-research/archive-hashes) repo.  
`SmashArcNetCLI <Hashes_all.txt> <data.arc>`

## Building
Clone with `git clone https://github.com/ScanMountGoat/SmashArcNet.git --recursive` to properly initialize all submodules.
A Rust installation is required, which can be downloaded from https://www.rust-lang.org/tools/install. 
Build with MSBuild or using Visual Studio 2019 or later.
