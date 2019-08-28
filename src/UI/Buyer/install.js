const { exec } = require('child_process');
const package = process.env.npm_config_custom_component_package;
const install = `npm install ${package}`;
const program = exec(install);

console.log("> " + install);
console.log("Running...");
program.stdout.on('data', console.log);
program.stderr.on('data', console.log);