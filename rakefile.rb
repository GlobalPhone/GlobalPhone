require 'visual_studio_files.rb'
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
  output_directory_tools = File.join(dir,"nuget_tool/tools/")
  mkdir_p output_directory_tools
  cp Dir.glob("./src/GlobalPhoneDbgen/bin/Debug/*.dll"), output_directory_tools
  cp Dir.glob("./src/GlobalPhoneDbgen/bin/Debug/GlobalPhoneDbgen.exe"), output_directory_tools
end

desc "test with nunit"
task :test => :build do
  command = Dir.glob(File.join(dir,"src/packages/NUnit.Runners.*/Tools/nunit-console.exe")).first
  assemblies = "GlobalPhone.Tests.dll"
  cd "src/GlobalPhone.Tests/bin/Debug" do
    sh "#{command} #{assemblies}"
  end
end

desc "create the nuget package"
task :nugetpack => [:core_nugetpack]

task :core_nugetpack => [:core_copy_to_nuspec] do |nuget|
  cd File.join(dir,"nuget") do
    sh "..\\src\\.nuget\\NuGet.exe pack GlobalPhone.nuspec"
  end
  cd File.join(dir,"nuget_tool") do
    sh "..\\src\\.nuget\\NuGet.exe pack GlobalPhoneDbgen.nuspec"
  end

end

desc "Install missing NuGet packages."
exec :install_packages do |cmd|
  FileList["src/**/packages.config"].each do |filepath|
    cmd.command = "./src/.nuget/NuGet.exe"
    cmd.parameters = "i #{filepath} -o ./src/packages"
  end
end

desc "regenerate links in dbgen"
task :regen_links_dbgen do
  global_phone = VisualStudioFiles::CsProj.new(File.open(File.join(dir,'src','GlobalPhone','GlobalPhone.csproj'), "r").read)
  global_phone_files = global_phone.files.select do |file|
    file.type=='Compile' && !file.file.end_with?('AssemblyInfo.cs')
  end
    
  global_phone_dbgen = VisualStudioFiles::CsProj.new(File.open(File.join(dir,'src','GlobalPhoneDbgen','GlobalPhoneDbgen.csproj'), "r").read)
  global_phone_dbgen.clear_links
  global_phone_files.each do |file|
    hash = file.to_hash
    hash[:file] = "..\\GlobalPhone\\#{file.file}"
    hash[:link] = "GlobalPhone\\#{file.file}"
    global_phone_dbgen.add(hash)
  end
  File.open(File.join(dir,'src','GlobalPhoneDbgen','GlobalPhoneDbgen.csproj'), "w") do |f|
    global_phone_dbgen.write f
  end
end

namespace :mono do
  desc "build isop on mono"
  xbuild :build do |msb|
    solution_dir = File.join(File.dirname(__FILE__),'src')
    nuget_tools_path = File.join(solution_dir, '.nuget')
    msb.properties :configuration => :Debug, 
      :SolutionDir => solution_dir,
      :NuGetToolsPath => nuget_tools_path,
      :NuGetExePath => File.join(nuget_tools_path, 'NuGet.exe'),
      :PackagesDir => File.join(solution_dir, 'packages')
    msb.targets :rebuild
    msb.verbosity = 'quiet'
    msb.solution = File.join('.','src',"GlobalPhone.sln")
  end

  desc "test with nunit"
  task :test => :build do
    # does not work for some reason 
    command = "nunit-console4"
    assemblies = "GlobalPhone.Tests.dll"
    cd "src/GlobalPhone.Tests/bin/Debug" do
      sh "#{command} #{assemblies}"
    end
  end

  desc "Install missing NuGet packages."
  task :install_packages do |cmd|
    FileList["src/**/packages.config"].each do |filepath|
      sh "mono --runtime=v4.0.30319 ./src/.nuget/NuGet.exe i #{filepath} -o ./src/packages"
    end
  end

end
