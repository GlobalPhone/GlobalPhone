require 'albacore'

task :default => ['build']

dir = File.dirname(__FILE__)
desc "build using msbuild"
msbuild :build do |msb|
  msb.properties :configuration => :Debug
  msb.targets :Clean, :Rebuild
  msb.verbosity = 'quiet'
  msb.solution =File.join(dir,"src", "GlobalPhone.sln")
end

task :core_copy_to_nuspec => [:build] do
  output_directory_lib = File.join(dir,"nuget/lib/40/")
  mkdir_p output_directory_lib
  cp Dir.glob("./src/GlobalPhone/bin/Debug/GlobalPhone.dll"), output_directory_lib
  
end

desc "create the nuget package"
task :nugetpack => [:core_nugetpack]

task :core_nugetpack => [:core_copy_to_nuspec] do |nuget|
  cd File.join(dir,"nuget") do
    sh "..\\src\\.nuget\\NuGet.exe pack GlobalPhone.nuspec"
  end
end

desc "Install missing NuGet packages."
exec :install_packages do |cmd|
  FileList["src/**/packages.config"].each do |filepath|
    cmd.command = "./src/.nuget/NuGet.exe"
    cmd.parameters = "i #{filepath} -o ./src/packages"
  end
end


