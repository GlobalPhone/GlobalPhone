require 'visual_studio_files.rb'
require 'albacore'
# .\src\GlobalPhoneDbgen\bin\Debug\GlobalPhoneDbgen.exe  > .\src\GlobalPhone.Tests\fixtures\record_data_hash.json
# .\src\GlobalPhoneDbgen\bin\Debug\GlobalPhoneDbgen.exe --test > .\src\GlobalPhone.Tests\fixtures\example_numbers.json

require 'rbconfig'
require 'nuget_helper'
$sln = File.join(File.dirname(__FILE__), "src", "GlobalPhone.sln")
$dir = File.dirname(__FILE__)

desc "build"
build :build do |msb|
  msb.prop :configuration, :Debug
  msb.prop :platform, "Mixed Platforms"
  msb.target = :Rebuild
  msb.be_quiet
  msb.nologo
  msb.sln = $sln
end

build :build_release do |msb|
  msb.prop :configuration, :Release
  msb.prop :platform, "Mixed Platforms"
  msb.target = :Rebuild
  msb.be_quiet
  msb.nologo
  msb.sln = $sln
end

desc "test using nunit console"
test_runner :test => [:build] do |nunit|
  nunit.exe = NugetHelper.nunit_path
  files = [File.join(File.dirname(__FILE__),"src","GlobalPhone.Tests","bin","Debug","GlobalPhone.Tests.dll")]
  nunit.files = files 
end

task :clean_nuget do
  cd File.join($dir,"nuget") do
    rm_rf "lib"
  end
end

task :main_copy_to_nuspec => [:clean_nuget, :build_release] do
  output_directory_lib = File.join($dir,"nuget/lib/40/")
  mkdir_p output_directory_lib
  cp Dir.glob("./src/GlobalPhone/bin/Release/GlobalPhone.*"), output_directory_lib
end

task :tool_copy_to_nuspec => [:clean_nuget, :build_release] do
  output_directory_tools = File.join($dir,"nuget_tool/tools/")
  mkdir_p output_directory_tools
  cp Dir.glob("./src/GlobalPhoneDbgen/bin/Release/*.dll"), output_directory_tools
  cp Dir.glob("./src/GlobalPhoneDbgen/bin/Release/GlobalPhoneDbgen.exe"), output_directory_tools
end

task :main_pack => [:main_copy_to_nuspec] do |nuget|
  cd File.join($dir,"nuget") do
    NugetHelper.exec "pack GlobalPhone.nuspec"
  end
end

task :tool_pack => [:tool_copy_to_nuspec] do |nuget|
  cd File.join($dir,"nuget_tool") do
    NugetHelper.exec "pack GlobalPhoneDbgen.nuspec"
  end
end

desc "create the nuget package"
task :pack => [:main_pack, :tool_pack]

desc "Install missing NuGet packages."
task :install_packages do
  NugetHelper.exec("restore ./src/GlobalPhone.sln -source http://www.nuget.org/api/v2/")
end

desc "regenerate links"
task :regen_links => [:regen_links_dbgen]

desc "regenerate links in dbgen"
task :regen_links_dbgen do
  global_phone = VisualStudioFiles::CsProj.new(File.open(File.join($dir,'src','GlobalPhone','GlobalPhone.csproj'), "r").read)
  global_phone_files = global_phone.files.select do |file|
    file.type=='Compile' && !file.file.end_with?('AssemblyInfo.cs')
  end
    
  global_phone_dbgen = VisualStudioFiles::CsProj.new(File.open(File.join($dir,'src','GlobalPhoneDbgen','GlobalPhoneDbgen.csproj'), "r").read)
  global_phone_dbgen.clear_links
  global_phone_files.each do |file|
    hash = file.to_hash
    hash[:file] = "..\\GlobalPhone\\#{file.file}"
    hash[:link] = "GlobalPhone\\#{file.file}"
    global_phone_dbgen.add(hash)
  end
  File.open(File.join($dir,'src','GlobalPhoneDbgen','GlobalPhoneDbgen.csproj'), "w") do |f|
    global_phone_dbgen.write f
  end
end

