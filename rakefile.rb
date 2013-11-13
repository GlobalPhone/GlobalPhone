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
