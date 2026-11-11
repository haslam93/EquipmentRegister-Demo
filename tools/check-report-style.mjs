#!/usr/bin/env node
// Checks HTML report templates against the report style guide (docs/report-style-guide.md).
// Usage: node tools/check-report-style.mjs [--all] [file ...]
//   --all   also check templates listed under "exempt" in tools/report-style.config.json
import { existsSync, readFileSync, readdirSync } from 'node:fs';
import { basename, dirname, join, relative, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';

const root = resolve(dirname(fileURLToPath(import.meta.url)), '..');
const config = JSON.parse(readFileSync(join(root, 'tools', 'report-style.config.json'), 'utf8'));

const args = process.argv.slice(2);
const checkExempt = args.includes('--all');
const requested = args.filter((arg) => !arg.startsWith('--'));

const palette = Object.entries(config.palette).map(([name, hex]) => ({ name, hex: hex.toUpperCase() }));
const fontStacks = config.fontStacks.map(normalizeFont);
const colourProperty = /^(color|background|background-color|outline(-color)?|border(-(top|right|bottom|left))?(-color)?)$/;
const namedColours = new Set([
  'black', 'white', 'red', 'green', 'blue', 'yellow', 'purple', 'orange', 'pink', 'gray', 'grey',
  'navy', 'teal', 'maroon', 'olive', 'lime', 'aqua', 'cyan', 'magenta', 'fuchsia', 'silver',
  'brown', 'gold', 'violet', 'indigo',
]);

function normalizeFont(value) {
  return value.toLowerCase().replace(/["']/g, '').replace(/\s*,\s*/g, ',').trim();
}

function expandHex(hex) {
  const digits = hex.slice(1).toUpperCase();
  return '#' + (digits.length === 3 ? [...digits].map((digit) => digit + digit).join('') : digits);
}

function nearestPaletteColour(hex) {
  const channels = (value) => [1, 3, 5].map((start) => parseInt(value.slice(start, start + 2), 16));
  const [r, g, b] = channels(hex);
  let best = palette[0];
  let bestDistance = Infinity;
  for (const entry of palette) {
    const [pr, pg, pb] = channels(entry.hex);
    const distance = (r - pr) ** 2 + (g - pg) ** 2 + (b - pb) ** 2;
    if (distance < bestDistance) {
      best = entry;
      bestDistance = distance;
    }
  }
  return `${best.name} ${best.hex}`;
}

function checkDeclaration(property, value, report) {
  if (property === 'font-family' && !fontStacks.includes(normalizeFont(value))) {
    report('RS001', `font-family ${value} is not an approved font stack (use: ${config.fontStacks[0]})`);
  }

  if (property === 'font-size') {
    const size = /^(\d+(?:\.\d+)?)([a-z%]*)$/i.exec(value);
    if (!size || size[2].toLowerCase() !== 'pt') {
      report('RS003', `font-size ${value} must be set in pt`);
    } else if (Number(size[1]) < config.minFontSizePt) {
      report('RS003', `font-size ${value} is below the ${config.minFontSizePt}pt minimum`);
    }
  }

  for (const match of value.matchAll(/#[0-9a-f]{6}\b|#[0-9a-f]{3}\b/gi)) {
    const hex = expandHex(match[0]);
    if (!palette.some((entry) => entry.hex === hex)) {
      report('RS002', `colour ${match[0]} is not in the report palette (nearest: ${nearestPaletteColour(hex)})`);
    }
  }

  if (/\b(rgba?|hsla?)\(/i.test(value)) {
    report('RS002', `colour ${value} must be a palette hex value, not rgb()/hsl()`);
  }

  if (colourProperty.test(property)) {
    for (const word of value.toLowerCase().split(/[^a-z]+/)) {
      if (namedColours.has(word)) {
        report('RS002', `named colour "${word}" is not allowed; use a palette hex value`);
      }
    }
  }
}

function checkDeclarations(text, report) {
  for (const declaration of text.split(';')) {
    const separator = declaration.indexOf(':');
    if (separator > 0) {
      const property = declaration.slice(0, separator).trim().toLowerCase();
      const value = declaration.slice(separator + 1).trim();
      if (/^[a-z-]+$/.test(property) && value) {
        checkDeclaration(property, value, report);
      }
    }
  }
}

function checkFile(path) {
  const problems = [];
  const html = readFileSync(path, 'utf8');
  const lines = html.split(/\r?\n/);
  let inStyleBlock = false;
  let depth = 0;

  lines.forEach((line, index) => {
    const report = (rule, message) => problems.push({ line: index + 1, rule, message });

    for (const inline of line.matchAll(/\sstyle\s*=\s*"([^"]*)"/gi)) {
      report('RS004', 'inline style attribute; move the styling into the <style> block');
      checkDeclarations(inline[1], report);
    }

    let css = line;
    if (!inStyleBlock) {
      const open = /<style[^>]*>/i.exec(line);
      if (!open) {
        return;
      }
      inStyleBlock = true;
      css = line.slice(open.index + open[0].length);
    }

    const close = css.search(/<\/style>/i);
    if (close >= 0) {
      css = css.slice(0, close);
      inStyleBlock = false;
    }

    let declarations = '';
    for (const char of css) {
      if (char === '{') {
        depth++;
        declarations += ';';
      } else if (char === '}') {
        depth = Math.max(0, depth - 1);
        declarations += ';';
      } else if (depth > 0) {
        declarations += char;
      }
    }
    checkDeclarations(declarations, report);
  });

  for (const block of config.requiredBlocks) {
    if (!new RegExp(`class\\s*=\\s*"[^"]*\\b${block}\\b`, 'i').test(html)) {
      problems.push({ line: 1, rule: 'RS005', message: `missing the standard block with class="${block}"` });
    }
  }

  return problems.sort((a, b) => a.line - b.line);
}

const templateFolder = join(root, config.templates);
const files = requested.length > 0
  ? requested.map((file) => resolve(file))
  : readdirSync(templateFolder).filter((name) => name.endsWith('.html')).sort().map((name) => join(templateFolder, name));

let problemCount = 0;
let failedFiles = 0;
let checkedFiles = 0;
const skipped = [];

for (const file of files) {
  const label = relative(root, file).replaceAll('\\', '/');
  if (!existsSync(file)) {
    console.error(`${label}: file not found`);
    process.exit(2);
  }

  if (!checkExempt && requested.length === 0 && config.exempt.includes(basename(file))) {
    skipped.push(basename(file));
    continue;
  }

  checkedFiles++;
  const problems = checkFile(file);
  if (problems.length > 0) {
    failedFiles++;
    problemCount += problems.length;
    console.log(label);
    for (const problem of problems) {
      console.log(`  ${String(problem.line).padStart(4)}  ${problem.rule}  ${problem.message}`);
    }
    console.log('');
  }
}

const exemptNote = skipped.length > 0 ? ` (${skipped.length} exempt: ${skipped.join(', ')})` : '';
if (problemCount > 0) {
  console.log(`FAIL  ${problemCount} problem(s) in ${failedFiles} of ${checkedFiles} report template(s)${exemptNote}`);
  console.log('See docs/report-style-guide.md for the approved fonts, sizes and palette.');
  process.exit(1);
}

console.log(`OK  ${checkedFiles} report template(s) follow the style guide${exemptNote}`);
